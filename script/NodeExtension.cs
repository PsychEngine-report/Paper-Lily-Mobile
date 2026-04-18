using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using LacieEngine.API;

namespace LacieEngine.Core
{
	public static class NodeExtension
	{
		public static Node[] GetSiblings(this Node node)
		{
			Godot.Collections.Array children = node.GetParent().GetChildren();
			children.Remove(node);
			return children.ToArray<Node>();
		}

		public static void AddFirst(this Node node, Node childNode)
		{
			node.AddChild(childNode);
			node.MoveChild(childNode, 0);
		}

		public static void AddChildDeferred(this Node node, Node nodeToAdd)
		{
			node.CallDeferred("add_child", nodeToAdd);
		}

		public static void Delete(this Node node)
		{
			node.GetParent().RemoveChild(node);
			node.QueueFree();
		}

		public static void DeleteIfValid(this Node node)
		{
			if (node.IsValid())
			{
				node.Delete();
			}
		}

		public static void Clear(this Node node)
		{
			foreach (Node child in node.GetChildren())
			{
				node.RemoveChild(child);
				child.QueueFree();
			}
		}

		public static void Reparent(this Node node, Node newParent)
		{
			node.GetParent().RemoveChild(node);
			newParent.AddChild(node);
		}

		public static void Pause(this Node node)
		{
			node.SetProcess(enable: false);
			node.SetPhysicsProcess(enable: false);
			node.SetProcessInput(enable: false);
			node.SetProcessInternal(enable: false);
			node.SetProcessUnhandledInput(enable: false);
			node.SetProcessUnhandledKeyInput(enable: false);
			foreach (Node child in node.GetChildren())
			{
				child.Pause();
			}
		}

		public static bool IsValid(this Node node)
		{
			if (node != null && Godot.Object.IsInstanceValid(node))
			{
				return node.IsInsideTree();
			}
			return false;
		}

		public static bool IsEmpty(this Node node)
		{
			return node.GetChildCount() < 1;
		}

		public static async Task DelaySeconds(this Node baseNode, double seconds)
		{
			await baseNode.ToSignal(baseNode.GetTree().CreateTimer((float)seconds, pauseModeProcess: false), "timeout");
		}

		public static T EnsureNode<T>(this Node node, string name) where T : Node, new()
		{
			T result = node.GetNodeOrNull<T>(name);
			if (result == null)
			{
				result = GDUtil.MakeNode<T>(name);
				node.AddChild(result);
			}
			return result;
		}

		public static List<T> FindChildren<T>(this Node node) where T : Node
		{
			List<T> foundChildren = new List<T>();
			CollectTypedChildrenRecursive(node, foundChildren);
			return foundChildren;
			static void CollectTypedChildrenRecursive(Node rootNode, List<T> collectedNodes)
			{
				foreach (Node node2 in rootNode.GetChildren())
				{
					if (node2 is T typedNode)
					{
						collectedNodes.Add(typedNode);
					}
					if (node2.GetChildCount() > 0)
					{
						CollectTypedChildrenRecursive(node2, collectedNodes);
					}
				}
			}
		}

		public static void InjectNodes(this Node node)
		{
			FieldInfo[] fields = node.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
			foreach (FieldInfo field in fields)
			{
				if (Attribute.IsDefined(field, typeof(GetNode)))
				{
					Node injectedNode = node.GetNode(field.GetCustomAttribute<GetNode>().Path);
					field.SetValue(node, injectedNode);
					if (injectedNode is INodeWithInjections)
					{
						Injector.Resolve(injectedNode);
						injectedNode.InjectNodes();
					}
				}
			}
		}
	}
}

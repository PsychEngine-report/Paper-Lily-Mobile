using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LacieEngine.Core;

namespace LacieEngine.Translations
{
	public class TranslatableMessages : IEnumerable
	{
		private List<LanguageCaption> _entries = new List<LanguageCaption>();

		public string Name { get; set; }

		public int Count => _entries.Count;

		public bool IsEmpty()
		{
			return _entries.Count == 0;
		}

		public void Add(string msgid)
		{
			Add(new LanguageCaption(msgid));
		}

		public void Add(string msgid, string msgctxt)
		{
			Add(new LanguageCaption(msgid, string.Empty, msgctxt));
		}

		public void Add(LanguageCaption caption)
		{
			_entries.Add(caption);
		}

		public string Get(string msgid, string msgctxt = null)
		{
			if (!msgctxt.IsNullOrEmpty())
			{
				foreach (LanguageCaption caption in _entries)
				{
					if (msgid == caption.Id && msgctxt == caption.Context)
					{
						return caption.Str;
					}
				}
			}
			foreach (LanguageCaption caption2 in _entries)
			{
				if (msgid == caption2.Id)
				{
					return caption2.Str;
				}
			}
			return null;
		}

		public TranslatableMessages()
		{
		}

		public TranslatableMessages(string name)
		{
			Name = name;
		}

		public void Clean()
		{
			for (int i = 0; i < _entries.Count; i++)
			{
				if (!MsgIdAlreadyExists(_entries[i].Id, i) && !_entries[i].Context.IsNullOrEmpty())
				{
					_entries[i] = new LanguageCaption(_entries[i].Id, _entries[i].Str);
				}
			}
			for (int j = 0; j < _entries.Count; j++)
			{
				if (EntryDuplicateExists(_entries[j], j))
				{
					Log.Warn("Duplicate entry: ", _entries[j].Id, " in ", Name);
				}
			}
			int count = _entries.Count;
			_entries = _entries.Distinct().ToList();
			if (count > _entries.Count)
			{
				Log.Warn("Duplicate captions were consumed: ", count - _entries.Count, " in ", Name);
			}
			bool EntryDuplicateExists(LanguageCaption entry, int msgidx)
			{
				for (int k = 0; k < _entries.Count; k++)
				{
					if (k != msgidx && _entries[k].Equals(entry))
					{
						return true;
					}
				}
				return false;
			}
			bool MsgIdAlreadyExists(string msgid, int msgidx)
			{
				for (int k = 0; k < _entries.Count; k++)
				{
					if (k != msgidx && _entries[k].Id == msgid)
					{
						return true;
					}
				}
				return false;
			}
		}

		public void RemoveContext()
		{
			for (int i = 0; i < _entries.Count; i++)
			{
				if (!_entries[i].Context.IsNullOrEmpty())
				{
					_entries[i] = new LanguageCaption(_entries[i].Id, _entries[i].Str);
				}
			}
			_entries = _entries.Distinct().ToList();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _entries.GetEnumerator();
		}
	}
}

using System;
using System.Collections.Generic;

using KeePass.Plugins;
using KeePass.UI;

using KeePassLib;
using KeePassLib.Utility;

namespace KeePassPacupColumns
{
	/// <summary>
	/// Provides the Part, Attn, Caty, User, and Pins columns.
	/// Each column reads a custom string field from the entry.
	/// Edit it in the entry dialog, 'Advanced' tab, 'String fields'.
	/// </summary>
	internal sealed class PacupColumnProvider : ColumnProvider
	{
		// Column titles.
		internal const string ColPart = "Part";
		internal const string ColAttn = "Attn";
		internal const string ColCaty = "Caty";
		internal const string ColUser = "User";
		internal const string ColPins = "Pins";

		// Custom string field names used to store the data.
		internal const string FieldPart = "Part";
		internal const string FieldAttn = "Attn";
		internal const string FieldCaty = "Caty";
		internal const string FieldUser = "User";
		internal const string FieldPins = "Pins";

		private const string PinMask = "********";
		private const string AttnMark = "!";

		private readonly IPluginHost m_host;

		private readonly string[] m_vColNames = new string[] {
			ColPart, ColAttn, ColCaty, ColUser, ColPins
		};

		public PacupColumnProvider(IPluginHost host)
		{
			m_host = host;
		}

		public bool ShowPins { get; set; }

		public override string[] ColumnNames
		{
			get { return m_vColNames; }
		}

		public override string GetCellData(string strColumnName, PwEntry pe)
		{
			if(strColumnName == null) { Debug.Assert(false); return string.Empty; }
			if(pe == null) { Debug.Assert(false); return string.Empty; }

			switch(strColumnName)
			{
				case ColPart:
					return ResolveParent(pe.Strings.ReadSafe(FieldPart));

				case ColAttn:
					return FormatAttn(pe.Strings.ReadSafe(FieldAttn));
				
				case ColCaty:
					return pe.Strings.ReadSafe(FieldCaty);

				case ColUser:
					return pe.Strings.ReadSafe(FieldUser);

				case ColPins:
					string strPin = pe.Strings.ReadSafe(FieldPins);
					if(strPin.Length == 0) return string.Empty;
					return (this.ShowPins ? strPin : PinMask);

				default:
					Debug.Assert(false);
					return string.Empty;
			}
		}

		public override bool SupportsCellAction(string strColumnName)
		{
			return false;
		}

		/// <summary>
		/// Boolean-like values (1, true, yes, y, x) are shown as a mark so
		/// that flagged entries stand out; any other text is shown as is.
		/// </summary>
		private static string FormatAttn(string strValue)
		{
			if(string.IsNullOrEmpty(strValue)) return string.Empty;

			string s = strValue.Trim().ToLowerInvariant();
			if(s.Length == 0) return string.Empty;
			if((s == "0") || (s == "false") || (s == "no") || (s == "n"))
				return string.Empty;
			if((s == "1") || (s == "true") || (s == "yes") || (s == "y") || (s == "x"))
				return AttnMark;

			return strValue.Trim();
		}

		/// <summary>
		/// The Part field may hold the parent entry's
		/// UUID (32 hex digits, as in a {REF:...} reference) or simply a title.
		/// A UUID that matches an entry in the open database is shown as
		/// that entry's title; anything else is displayed unchanged.
		/// </summary>
		private string ResolveParent(string strValue)
		{
			if(string.IsNullOrEmpty(strValue)) return string.Empty;

			string str = strValue.Trim();
			if(str.Length != 32) return str;

			try
			{
				PwDatabase pd = m_host.Database;
				if((pd == null) || !pd.IsOpen) return str;

				byte[] pbUuid = MemUtil.HexStringToByteArray(str);
				if((pbUuid == null) || (pbUuid.Length != 16)) return str;

				PwEntry peParent = pd.RootCategory.FindEntry(new PwUuid(pbUuid), true);
				if(peParent == null) return str;

				return peParent.Strings.ReadSafe(PwDefs.TitleField);
			}
			catch(Exception) { }

			return str;
		}
	}
}

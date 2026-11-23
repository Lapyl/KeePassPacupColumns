using System;
using System.Windows.Forms;

using KeePass.Plugins;

namespace KeePassPacupColumns
{
	/// <summary>
	/// Plug-in entry point.
	/// KeePass requires the class to be named "&lt;namespace&gt;.&lt;namespace&gt;Ext"
	/// and to derive from Plugin.
	/// </summary>
	public sealed class KeePassPacupColumnsExt : Plugin
	{
		private IPluginHost m_host = null;
		private PacupColumnProvider m_prov = null;
		private ToolStripMenuItem m_tsmiShowPins = null;

		public override string UpdateUrl
		{
			// Set this to a version-information file once you publish the plug-in.
			get { return null; }
		}

		public override bool Initialize(IPluginHost host)
		{
			if(host == null) return false;
			m_host = host;

			m_prov = new PacupColumnProvider(m_host);
			m_host.ColumnProviderPool.Add(m_prov);

			// Tools menu toggle: PIN values are masked by default.
			m_tsmiShowPins = new ToolStripMenuItem();
			m_tsmiShowPins.Text = "Show PINs in the 'Pin' Column";
			m_tsmiShowPins.CheckOnClick = true;
			m_tsmiShowPins.Click += this.OnToggleShowPins;
			m_host.MainWindow.ToolsMenu.DropDownItems.Add(m_tsmiShowPins);

			return true;
		}

		public override void Terminate()
		{
			if(m_host == null) return;

			if(m_tsmiShowPins != null)
			{
				m_tsmiShowPins.Click -= this.OnToggleShowPins;
				m_host.MainWindow.ToolsMenu.DropDownItems.Remove(m_tsmiShowPins);
				m_tsmiShowPins.Dispose();
				m_tsmiShowPins = null;
			}

			m_host.ColumnProviderPool.Remove(m_prov);
			m_prov = null;
			m_host = null;
		}

		private void OnToggleShowPins(object sender, EventArgs e)
		{
			if((m_prov == null) || (m_tsmiShowPins == null)) return;

			m_prov.ShowPins = m_tsmiShowPins.Checked;
			m_host.MainWindow.UpdateUI(false, null, false, null, true, null, false);
		}
	}
}

using System;
using System.Collections.Generic;
using MonoMac.AppKit;
using MonoMac.Foundation;


namespace VisualCompiler
{
	public class CppFileDataSource: NSComboBoxDataSource
	{
		readonly List<FileItem> _filenames;

		public CppFileDataSource(List<FileItem> filenames)
		{
			this._filenames = filenames;
		}

		public override string CompletedString (NSComboBox comboBox, string uncompletedString)
		{
			return _filenames.Find (n => n.Name.StartsWith (uncompletedString, StringComparison.InvariantCultureIgnoreCase)).Name;
		}

		public override int ItemCount (NSComboBox comboBox)
		{
			return _filenames.Count;
		}

		public override NSObject ObjectValueForItem (NSComboBox comboBox, int index)
		{
			return NSObject.FromObject ((_filenames [index]).Name);
		}



	}

}
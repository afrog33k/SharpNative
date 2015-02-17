// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace VisualCompiler
{
	[Register ("MyDocument")]
	partial class MyDocument
	{
		[Outlet]
		public MonoMac.AppKit.NSTextView ConsoleText { get; private set; }

		[Outlet]
		public MonoMac.AppKit.NSComboBox CppFileList { get; private set; }

		[Outlet]
		public MonoMac.AppKit.NSTextField CPPFilename { get; private set; }

		[Outlet]
		public MonoMac.AppKit.NSButton CPPRunButton { get; private set; }

		[Outlet]
		public MonoMac.AppKit.NSTextView CPPTextEditor { get; private set; }

		[Outlet]
		MonoMac.AppKit.NSTextField CSharpFilename { get; set; }

		[Outlet]
		public MonoMac.AppKit.NSButton CSharpRunButton { get; private set; }

		[Outlet]
		public MonoMac.AppKit.NSTextView CSharpTextEditor { get; private set; }

		[Outlet]
		public MonoMac.AppKit.NSTextView IntermediateText { get; private set; }

		[Outlet]
		MonoMac.AppKit.NSButton OpenFileButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton RunAllTests { get; set; }

		[Outlet]
		public MonoMac.AppKit.NSButton TestButton { get; private set; }

		[Outlet]
		public MonoMac.AppKit.NSButton TestStatusButton { get; private set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (RunAllTests != null) {
				RunAllTests.Dispose ();
				RunAllTests = null;
			}

			if (ConsoleText != null) {
				ConsoleText.Dispose ();
				ConsoleText = null;
			}

			if (CppFileList != null) {
				CppFileList.Dispose ();
				CppFileList = null;
			}

			if (CPPFilename != null) {
				CPPFilename.Dispose ();
				CPPFilename = null;
			}

			if (CPPRunButton != null) {
				CPPRunButton.Dispose ();
				CPPRunButton = null;
			}

			if (CPPTextEditor != null) {
				CPPTextEditor.Dispose ();
				CPPTextEditor = null;
			}

			if (CSharpFilename != null) {
				CSharpFilename.Dispose ();
				CSharpFilename = null;
			}

			if (CSharpRunButton != null) {
				CSharpRunButton.Dispose ();
				CSharpRunButton = null;
			}

			if (CSharpTextEditor != null) {
				CSharpTextEditor.Dispose ();
				CSharpTextEditor = null;
			}

			if (IntermediateText != null) {
				IntermediateText.Dispose ();
				IntermediateText = null;
			}

			if (OpenFileButton != null) {
				OpenFileButton.Dispose ();
				OpenFileButton = null;
			}

			if (TestButton != null) {
				TestButton.Dispose ();
				TestButton = null;
			}

			if (TestStatusButton != null) {
				TestStatusButton.Dispose ();
				TestStatusButton = null;
			}
		}
	}
}

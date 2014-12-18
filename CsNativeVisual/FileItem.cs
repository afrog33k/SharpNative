namespace CsNativeVisual
{
    public class  FileItem
    {
        public   string Name { get; set; }
        public string Location { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
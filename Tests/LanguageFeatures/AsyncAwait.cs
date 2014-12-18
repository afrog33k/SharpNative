//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System; 
using System.Text; 


public class Driver {
   
    public void Main() {
        jQuery.Select("#main h2").Click(async (el, evt) => {
            await jQuery.Select("#main p").FadeOutTask();
            await jQuery.Select("#main p").FadeInTask();
            Window.Alert("Done");
        });
    
       // Console.WriteLine(sb.ToString());
    }
}
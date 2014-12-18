//GLTest only works on mac osx (tested 10.10)
using System;
using System.Runtime.InteropServices;

namespace OpengGLTest
{

    class MainClass
    {


        const string openGLLib= "/System/Library/Frameworks/OpenGL.framework/OpenGL";
        const string glutLib = "/System/Library/Frameworks/GLUT.framework/GLUT";


        static int GLUT_RGB     =   0;
        static int GLUT_RGBA        =   0;
        static int GLUT_DOUBLE  =       2;
        static int GLUT_DEPTH       =   16;
        static int GL_DEPTH_TEST  =                  0x0B71;
        static int GL_SMOOTH      =                  0x1D01;
        static int GL_COLOR_BUFFER_BIT           =    0x00004000;
        static int GL_DEPTH_BUFFER_BIT            =   0x00000100;
        static int GL_TRIANGLES           =           0x0004;
        static int GL_PROJECTION           =          0x1701;
        static int GL_MODELVIEW            =             0x1700;

        [DllImport(openGLLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern  void glViewport(int x,int y,int w,int h) ;
        [DllImport(openGLLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern  void glClearColor(float a,float r,float g,float b) ;
        [DllImport(openGLLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern  void glEnable(int op) ;
        [DllImport(openGLLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern  void glShadeModel(int op) ;
        [DllImport(openGLLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern  void glClear(int op) ;
        [DllImport(openGLLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern  void glLoadIdentity() ;
        [DllImport(openGLLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern  void glBegin(int op) ;
        [DllImport(openGLLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern  void glEnd() ;
        [DllImport(openGLLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern  void glVertex3f(float a,float b,float c);
        [DllImport(openGLLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern  void glMatrixMode(int a) ;
        [DllImport(openGLLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern  void glFrustum(double a,double b,double c,double d,double e,double f);


        [DllImport(glutLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern      void glutInit(ref int a,string[] b);
        [DllImport(glutLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern      void glutInitDisplayMode(int a);
        [DllImport(glutLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern      void glutInitWindowSize(int a,int b);
        [DllImport(glutLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern      void glutInitWindowPosition(int a,int b);
        [DllImport(glutLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern      void glutCreateWindow([MarshalAs(UnmanagedType.LPStr)] string  a);
        [DllImport(glutLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern      void glutMainLoop();
        [DllImport(glutLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern      void glutDisplayFunc(Func1 g);
        [DllImport(glutLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern      void glutIdleFunc(Func1 g);
        [DllImport(glutLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern      void glutReshapeFunc(Func2 j);
        [DllImport(glutLib, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static  extern      void glutSwapBuffers() ;

        public delegate void Func1();
        public delegate void Func2(int x, int y);


        static  void display()
        {
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
            glLoadIdentity();

            glBegin(GL_TRIANGLES);
            glVertex3f(0.0f, 0.0f, -10.0f);
            glVertex3f(1.0f, 0.0f, -10.0f);
            glVertex3f(0.0f, 1.0f, -10.0f);
            glEnd();

            glutSwapBuffers();
        }

        static  void reshape(int w, int h)
        {
            Console.WriteLine(w);
            Console.WriteLine(h);

            glViewport(0, 0, w, h);
            glMatrixMode(GL_PROJECTION);
            glLoadIdentity();
            glFrustum(-0.1, 0.1, -(float)(h)/(10.0*(float)(w)), (float)(h)/(10.0*(float)(w)), 0.5, 1000.0);
            glMatrixMode(GL_MODELVIEW);
            glLoadIdentity();
        }




        public static void Main (string[] args)
        {
            Console.WriteLine ("Hello World!");
            int argc = 0;
            string[] argsi = new string[]{"yo","mehn"};
            glutInit(ref argc,argsi); // cannot pass null to glut

            glutInitDisplayMode(GLUT_DOUBLE | GLUT_DEPTH| GLUT_RGBA);

            glutInitWindowSize(512,512);

            glutInitWindowPosition(0,0);

            glutCreateWindow("Hello!");

            glClearColor(0.8f, 0.4f, 0.5f, 1.0f);
            glEnable(GL_DEPTH_TEST);
            glShadeModel(GL_SMOOTH);


            glutIdleFunc(display);
            glutDisplayFunc(display);
            glutReshapeFunc(reshape);
            glutMainLoop();
        }
    }
}

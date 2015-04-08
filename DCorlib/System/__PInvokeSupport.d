module System.__PInvokeSupport;
import core.stdc.stdio;
import std.stdio;
import core.stdc.stdio;
import core.stdc.stdlib;
import core.sys.posix.dlfcn;
import std.conv;
import std.string;
import System.Namespace;
//PInvoke Support

public static void __FreeNativeLibrary(void * handle)
{
	version(darwin)
	{
		dlclose(handle);
	}
}

static void*[wstring] __dllMap;

public static void *__LoadNativeLibrary(wstring libName)
{

	void** handleIn = libName in __dllMap;

	if(handleIn !is null)
		return __dllMap[libName];

	version(darwin)
	{
		void* handle = dlopen(cast(char*)(std.conv.to!(char[])(libName)), RTLD_LAZY);
		if (!handle)
		{
			//throw new Exception("dlopen error: " ~ dlerror());
			printf("dlopen error: %s\n", dlerror());
			exit(1);
		}
		//	writeln("successfully loaded " ~ libName);
		__dllMap[libName] = handle;
		return handle;
	}
	version(Windows)
	{
		import core.runtime, core.sys.windows.windows;
		void* handle = Runtime.loadLibrary(std.conv.to!(char[])(libName));
		if (!handle)
		{
			//throw new Exception("dlopen error: " ~ dlerror());
			printf("failed to load library");
			exit(1);
		}
		//writeln("successfully loaded " ~ libName);
		__dllMap[libName] = handle;
		return handle;

	}
	return null;
}

static void*[string] __dllFuncMap;

public static void *__LoadLibraryFunc(void* library, wstring funcName)
{
	version(darwin)
	{
		char* error = dlerror();
		auto func= dlsym(library, std.conv.to!(char[])(funcName).toStringz);
		if (error)
		{
			printf("dlsym error: %s - %s\n", error, cast(char*)"glutInit");
			exit(1);
		}

		return func;
	}
	version(Windows)
	{
		import core.runtime, core.sys.windows.windows;
		return GetProcAddress(library, std.conv.to!(char[])(funcName).toStringz);
	}
	return null;
}

static  void* __DllImportMap[wstring];

static void __SetupDllImport(wstring name)
{
	__DllImportMap[name] = __LoadNativeLibrary(name);
	//writeln(__DllImportMap[name]);
}

static void __FreeDllImports()
{
	foreach(lib ; __DllImportMap)
	{
		__FreeNativeLibrary(lib);
	}
}
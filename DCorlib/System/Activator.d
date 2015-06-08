module System.Activator;

import System.Namespace;

class Activator : NObject
{
	public static NObject CreateInstance(Type type, Array_T!(NObject) args) //Just testing
        {
			if (type is null)
                throw new ArgumentNullException(new String("type"));

			return type.Create();
          /*  return CreateInstance(type,
                                  Activator.ConstructorDefault,
                                  null,
                                  args,
                                  null,
                                  null);*/
        }

	public static NObject CreateInstance(Type type)
	{
		return type.Create();
	}

}

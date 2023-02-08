using System;

namespace Sonat
{
	  public class LogParameter
{
    public int order { get; }

    //        public string log { get; private set; }

    public enum ParamType
    {
        BooleanType,
        StringType,
        IntType,
        FloatType,
    }
    public string stringValue { get; }
    public bool boolValue { get; }
    public int intValue { get; }
    public float floatValue { get; }
    public string stringKey { get; }
    public ParamType type;

#if (dummy || global_dummy) && !use_firebase
    private void CreateFirebaseParam()
    {
       
    }
#else
    public Firebase.Analytics.Parameter Param;

    private void CreateFirebaseParam()
    {
        switch (type)
        {
            case ParamType.BooleanType:
                Param = new Firebase.Analytics.Parameter(stringKey, boolValue.ToString());
                break;
            case ParamType.StringType:
                Param = new Firebase.Analytics.Parameter(stringKey, stringValue);
                break;
            case ParamType.IntType:
                Param = new Firebase.Analytics.Parameter(stringKey, intValue);
                break;
            case ParamType.FloatType:
                Param = new Firebase.Analytics.Parameter(stringKey, floatValue);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
#endif

    public LogParameter(ParameterEnum name, string value, int order = 0)
    {
        this.order = order;
        type = ParamType.StringType;
        stringKey = name.ToString();
        stringValue = value;
        CreateFirebaseParam();
    }

    public LogParameter(ParameterEnum name, bool value, int order = 0)
    {
        this.order = order;
        type = ParamType.BooleanType;
        stringKey = name.ToString();
        boolValue = value;
        CreateFirebaseParam();
    }

    public LogParameter(ParameterEnum name, int value, int order = 0)
    {
        this.order = order;
        type = ParamType.IntType;
        stringKey = name.ToString();
        intValue = value;
        CreateFirebaseParam();
    }

    public LogParameter(ParameterEnum name, float value, int order = 0)
    {
        this.order = order;
        type = ParamType.FloatType;
        stringKey = name.ToString();
        floatValue = value;
        CreateFirebaseParam();
    }

    public LogParameter(string name, string value, int order = 0)
    {
        this.order = order;
        type = ParamType.StringType;
        stringKey = name;
        stringValue = value;
        CreateFirebaseParam();
    }

    public LogParameter(string name, int value, int order = 0)
    {
        this.order = order;
        type = ParamType.IntType;
        stringKey = name;
        intValue = value;
        CreateFirebaseParam();
    }

    public LogParameter(string name, bool value, int order = 0)
    {
        this.order = order;
        type = ParamType.BooleanType;
        stringKey = name;
        boolValue = value;
        CreateFirebaseParam();
    }

    public LogParameter(string name, float value, int order = 0)
    {
        this.order = order;
        type = ParamType.FloatType;
        stringKey = name;
        floatValue = value;
        CreateFirebaseParam();
    }
}

}

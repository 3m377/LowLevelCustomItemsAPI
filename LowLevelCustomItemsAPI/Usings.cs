#if EXILED
global using Log = Exiled.API.Features.Log;
#else
global using Log = LabApi.Features.Console.Logger;
#endif
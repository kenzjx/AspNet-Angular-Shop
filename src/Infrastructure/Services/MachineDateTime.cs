using Common;

namespace ClassLibrary1.Services;

public class MachineDateTime : IDateTime
{
    public DateTime Now => DateTime.Now;

    public int CurrentYear => DateTime.Now.Year;
}
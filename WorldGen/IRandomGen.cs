namespace WorldGen
{
    interface IRandomGen
    {
        int GenerateInt(int upper);
        int GenerateInt(int lower, int upper);
        double GenerateDouble();
    }
}

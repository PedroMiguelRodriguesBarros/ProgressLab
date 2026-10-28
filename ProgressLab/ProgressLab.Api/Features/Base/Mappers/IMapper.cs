public interface IMapper<TSrc, TDest>
{
    TDest Map(TSrc src);
}
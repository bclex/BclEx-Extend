namespace Contoso.Practices.TdsServer.Wire
{
typedef struct TdsVarbinary
{
	TDS_SMALLINT len;
	TDS_CHAR array[256];
} TDS_VARBINARY;

}


namespace Contoso.Practices.TdsServer.Wire
{
typedef struct TdsVarchar
{
	TDS_SMALLINT len;
	TDS_CHAR array[256];
} TDS_VARCHAR;


}


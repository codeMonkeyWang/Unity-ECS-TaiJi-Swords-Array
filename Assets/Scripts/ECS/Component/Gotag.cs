using Unity.Entities;
using Unity.Mathematics;

public struct GoTag : IComponentData
{
    //�ɽ�Ⱥ���뽣��Ҫ�����Ŀ���
    public float3 targetPos;

    //��Ӧ��TempEntity
    public Entity TempEntity;

    //ԭ��λ�ã����������ٶ�
    public float3 originPos;

    public bool isBack ;

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine.UI;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;
using Unity.Physics.Systems;


public class Test : MonoBehaviour
{
    public static Test Instance;

    public GameObject swordPrefab;
    public int swordGroupAmount;
    public float RotateSpeed;

    private EntityManager _manager;
    //blobAssetStore��һ���ṩ������࣬������������󴴽�ʱ���졣
    private BlobAssetStore _blobAssetStore;
    private GameObjectConversionSettings _settings;

    private Entity swordEntity;
    public Transform TargetPos;

    Unity.Physics.RaycastHit raycastHit;
    private bool isGo = false;
    EntityArchetype tempAchetype;
    void Start()
    {
        Instance = this;
        raycastHit = new Unity.Physics.RaycastHit();
        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _blobAssetStore = new BlobAssetStore();
        _settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
        swordEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(swordPrefab, _settings);

        ForbidSystem();

     tempAchetype = _manager.CreateArchetype(
      typeof(Translation),
      typeof(LocalToWorld),
      typeof(Rotation),
      typeof(RotateTag),
      typeof(Target),
      typeof(TempEntityTag)

      );
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            BurstGenerateSword();
        }

    }

    private void OnDestroy()
    {
        _blobAssetStore.Dispose();
    }

    #region ���ɷɽ�Entity ��TempEntity

    public void SpawnNewSword(float2 pos,Entity prefabEntity)
    {
        Entity newSword = _manager.Instantiate(swordEntity);
        Translation ballTrans = new Translation
        {
            Value = new float3(pos.x, 0f, pos.y)
        };

        float3 temp;
        float randomSpeed = UnityEngine.Random.Range(4f, 7f);
        temp = float3.zero;

        Target target = new Target
        {
            isGo = false,
            Tpos = temp,
            randomSpeed = randomSpeed,
            targetTempentity = prefabEntity
        };

        _manager.AddComponentData(newSword, ballTrans);
        _manager.AddComponentData(newSword, target);
    }


    private Entity SpawnTempEntity(float2 aa)
    {

        Entity tempEntity = _manager.CreateEntity(tempAchetype);

        Target target2 = new Target
        {
            isGo = false,
            Tpos = float3.zero,
        };
        
        Translation tempTrans = new Translation
        {
            Value = new float3(aa.x, 0f, aa.y)
        };

        _manager.SetComponentData(tempEntity, target2);
        _manager.SetComponentData(tempEntity, tempTrans);

        return tempEntity;

    } 
    #endregion


    public void ForbidSystem()
    {
        TestSystem system = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<TestSystem>();
        system.Enabled = false;
    }
    public void BurstGenerateSword()
    {
        Debug.Log("��������:" + GetPixel.Instance.posList.Count);
  
        //����λ���б����ɶ�Ӧ�����ķɽ�Entity
        for (int i = 0; i < GetPixel.Instance.posList.Count; i++)
        {
          Entity temp = SpawnTempEntity(GetPixel.Instance.posList[i]);
          SpawnNewSword(GetPixel.Instance.posList[i],temp );
        }
    }


    #region obsolete

    public Entity Raycast(float3 startPos, float3 endPos)
    {
        //���Ȼ�ȡ��������
        BuildPhysicsWorld physicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        //Ȼ���ȡ��ײ����
        CollisionWorld collisionWorld = physicsWorld.PhysicsWorld.CollisionWorld;

        RaycastInput raycastInput = new RaycastInput()
        {
            Start = startPos,
            End = endPos,
            //������ײ����������������ĳЩ�㼶�µ������Ƿ�������߼��
            Filter = CollisionFilter.Default,
            //Filter = new CollisionFilter()
            //{
            //    BelongsTo = ~0u,
            //    CollidesWith = ~0u,
            //    GroupIndex = 0,
            //}
        };

        //��������ȥ���Entityʵ�� 
        if (collisionWorld.CastRay(raycastInput, out raycastHit))
        {
            isGo = true;
            //�õ��������߻��е�entity
            Entity entity = physicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex].Entity;
            return entity;
        }
        else
        {
            Debug.Log("Notthing Found");
            return Entity.Null;
        }
    }

    // �ɽ�ǰ��Ŀ���
    public void GetTargetModelPos(float3 pos)
    {
        Debug.Log("11");
        EntityQueryDesc description = new EntityQueryDesc
        {
            None = new ComponentType[] { typeof(GoTag), typeof(TempEntityTag) },
            All = new ComponentType[] { typeof(Rotation), ComponentType.ReadOnly<SwordTag>() }
        };
        //��ȡ�ɽ�Ⱥ
        EntityQuery entityQuery = _manager.CreateEntityQuery(description);

        NativeArray<Entity> newgroupArray = entityQuery.ToEntityArray(Allocator.Persistent);

        if (newgroupArray.Length < swordGroupAmount)
        {
            Debug.Log("��ǰ���ȣ�" + newgroupArray.Length);
            entityQuery.Dispose();
            newgroupArray.Dispose();
            return;
        }

        for (int i = 0; i < swordGroupAmount; i++)
        {
            //Translation aaa = _manager.GetComponentData<Translation>(newgroupArray[i]);
            //Entity temp = SpawnTempEntity(aaa.Value);
            float randomSpeed = UnityEngine.Random.Range(6f, 10f);
            Target target = new Target
            {
                isGo = true,
                Tpos = pos,

                // entity=temp 
            };
        }
        entityQuery.Dispose();
        newgroupArray.Dispose();
    }

    #endregion
}

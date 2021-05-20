using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
public class GetPixel : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    //���ص����λ��
    public List<int2> posList;

    [Header("Drawing")]

    //�����ܶ�
    public int drawDensity ;
    //������ɢ�̶�
    public int disperseMin;
    public static GetPixel Instance;
    //ͼƬ���
    private int width;
    private int height;
    void Start()
    {
        Instance = this;
        width = spriteRenderer.sprite.texture.width;
        height = spriteRenderer.sprite.texture.height;
        Debug.Log("ͼƬ���" + width + "ͼƬ�߶�" + height);
        GetPixelPos();
     
    }

  
    public void GetPixelPos()
    {
        int halfHeight= height / 2;
        int halfWidth = width / 2;
        int2 tempPos;
        for (int i = 0; i < height; i += drawDensity)
        {
            for (int j = 0; j < width; j += drawDensity)
            {
                //��ȡÿ��λ�����ص����ɫ
                Color32 c = spriteRenderer.sprite.texture.GetPixel(j, i);
                tempPos.y = (j-halfHeight)*disperseMin; 
               // Debug.Log("RGBA:" + c);
               //�����Ӧλ����ɫ��Ϊ͸�������¼���굽List��
                if (c.a != 0)
                {
                    tempPos.x = (i-halfWidth)* disperseMin;
                    posList.Add(tempPos);
                }

            }
        }

        Debug.Log("λ��List����"+posList.Count);
    }
}

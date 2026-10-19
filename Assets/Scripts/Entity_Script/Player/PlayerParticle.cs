using UnityEngine;

enum ParticleType
{
    WALK,
    JUMPSTART,
    JUMPEND
}

public class PlayerParticle : MonoBehaviour
{
    [Header("걷기 파티클")]
    [SerializeField] ParticleSystem[] walkParticle;                                 //0번 나뭇잎, 1번 먼지(흙바닥, 나무바닥, 돌바닥 등등...)    

    [Header("점프 시작 파티클")]
    [SerializeField] ParticleSystem[] jumpStartParticle;

    [Header("점프 착지 파티클")]
    [SerializeField] ParticleSystem[] jumpEndParticle;

    PlayerStandTile Pstand;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Pstand = GetComponent<PlayerStandTile>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WalkParticleOn(Vector3 walkPos, Vector3 walkRo)
    {
        int parNum = Pstand.standTileNum;
        walkParticle[parNum].transform.localPosition = walkPos;
        walkParticle[parNum].transform.eulerAngles = walkRo;        
        walkParticle[parNum].Play();
    }

    public void JumpStartParticleOn()
    {
        jumpStartParticle[Pstand.standTileNum].Play();
    }

    public void JumpEndParticleOn()
    {
        jumpEndParticle[Pstand.standTileNum].Play();
    }
}

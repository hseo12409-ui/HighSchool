using System.Collections;
using UnityEngine;

public class ObjectMoveScript : MonoBehaviour
{
    [SerializeField] Vector3 exPos;
    [SerializeField] bool isUp;

    private Vector3 up;
    private Vector3 down;

    Coroutine UpCoroutine;
    Coroutine DownCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (isUp)
        {
            up = transform.position;
            down = transform.position - exPos;
            Debug.Log(down);
        }
        else
        {
            up = transform.position + exPos;
            down = transform.position;            
        }
    }

    public void posUpMethod()
    {
        if (UpCoroutine != null)
        {
            StopCoroutine(UpCoroutine);
        }
        if (DownCoroutine != null)
        {
            StopCoroutine(DownCoroutine);
        }
        UpCoroutine = StartCoroutine(PosUp());
    }

    public void posDownMethod()
    {
        if (UpCoroutine != null)
        {
            StopCoroutine(UpCoroutine);
        }
        if (DownCoroutine != null)
        {
            StopCoroutine(DownCoroutine);
        }
        DownCoroutine = StartCoroutine(PosDown());
    }

    public IEnumerator PosUp()
    {        
        while (Vector3.Distance(transform.position, up) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, up, 0.1f);            
            yield return new WaitForSeconds(0.01f);
        }
        UpCoroutine = null;
    }

    public IEnumerator PosDown()
    {        
        while (Vector3.Distance(transform.position, down) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, down, 0.1f);            
            yield return new WaitForSeconds(0.01f);
        }
        DownCoroutine = null;
    }
}

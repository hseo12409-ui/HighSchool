using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elesticblock : MonoBehaviour
{
    private Vector3 originalPos;
    [SerializeField] Vector3 pos;
    private Vector3 movePos;

    [SerializeField] List <GameObject> collisionObject = new List<GameObject>();

    bool comback;
    [SerializeField] bool combackinstant;

    Coroutine setPosvari;

    BoxCollider2D bc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        bc = GetComponent<BoxCollider2D>();

        originalPos = transform.parent.position;
        movePos = originalPos;        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!comback)
        {
            transform.parent.position = Vector3.Lerp(transform.parent.position, movePos, 0.35f);
        }else
        {
            transform.parent.position = Vector3.Lerp(transform.parent.position, movePos, 0.07f);
        }
    }

    IEnumerator setPos()
    {
        comback = false;
        movePos = originalPos + pos;
        if (combackinstant)
        {
            yield return new WaitForSeconds(0.7f);
            comback = true;
            movePos = originalPos;
        }
        setPosvari = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (setPosvari == null && collisionObject.Count == 0)
        {
            setPosvari = StartCoroutine(setPos());
            collisionObject.Add(collision.gameObject);
        }
        for (int i = 0; i < collisionObject.Count; i++)
        {
            if (collisionObject[i] != collision.gameObject)
            {
                collisionObject.Add(collision.gameObject);
                break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        for (int i = 0; i < collisionObject.Count; i++)
        {
            if (collisionObject[i] == collision.gameObject)
            {
                collisionObject.RemoveAt(i);
                break;
            }
        }
        if (!combackinstant && collisionObject.Count == 0)
        {
            comback = true;
            movePos = originalPos;
        }        
    }
}

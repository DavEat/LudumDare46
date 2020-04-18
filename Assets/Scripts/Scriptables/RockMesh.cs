using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RockMesh", menuName = "Data/RockMesh", order = 1)]
public class RockMesh : ScriptableObject
{
    [SerializeField] Mesh[] m_bigDamageSand = null;
    [SerializeField] Mesh[] m_bigDamageNoSand = null;

    [SerializeField] Mesh[] m_smallDamageSand = null;
    [SerializeField] Mesh[] m_smallDamageNoSand = null;

    public Mesh GetMesh(bool bigDamage, bool sand)
    {
        Mesh mesh;

        if (bigDamage)
        {
            if (sand)
            {
                int index = Random.Range(0, m_bigDamageSand.Length);
                mesh = m_bigDamageSand[index];
            }
            else
            {
                int index = Random.Range(0, m_bigDamageNoSand.Length);
                mesh = m_bigDamageNoSand[index];
            }
        }
        else
        {
            if (sand)
            {
                int index = Random.Range(0, m_smallDamageSand.Length);
                mesh = m_smallDamageSand[index];
            }
            else
            {
                int index = Random.Range(0, m_smallDamageNoSand.Length);
                mesh = m_smallDamageNoSand[index];
            }
        }
        return mesh;
    }
}

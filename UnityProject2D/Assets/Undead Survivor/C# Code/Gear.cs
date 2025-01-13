using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate;

    public void Init(ItemData data)
    {
        // 세팅
        name = "Gear " + data.itemID;
        // 부모 설정
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;

        // property set
        type = data.itemType;
        rate = data.damages[0];
        ApplyGear();

	}
    public void LevelUP(float rate)
    {
        this.rate = rate;
        ApplyGear();

	}
    void ApplyGear()
    {
        switch (type)
        {
            case ItemData.ItemType.Glove:
                RateUP();
                break;
            case ItemData.ItemType.Shoe:
                SpeedUP();
                break;
        }
    }
    void RateUP()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();
        foreach(Weapon weapon in weapons)
        {
            switch (weapon.id)
            {
                case 0:
                    weapon.speed = 150 + (150 * rate); break;
                default:
                    weapon.speed = 0.5f * (1f - rate); break;
            }
        }
    }
    void SpeedUP()
    {
        float speed = 3;
        GameManager.instance.player.Speed = speed + speed * rate;
    }
}

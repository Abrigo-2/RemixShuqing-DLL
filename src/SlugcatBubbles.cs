using System;
using RWCustom;
using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JollyCoop;
using MoreSlugcats;
using Watcher;

// Same as LizardBubble, but made compatible with PlayerGraphics. (slugcats!)
// Token: 0x02000215 RID: 533
public class PlayerBubble : CosmeticSprite
{
    // Token: 0x170003F7 RID: 1015
    // (get) Token: 0x060018B7 RID: 6327 RVA: 0x001C8673 File Offset: 0x001C6873
    public float Stickiness
    {
        get
        {
            return Mathf.Max(this.stickiness, Mathf.InverseLerp(4f, 14f, this.playerGraphics.head.vel.magnitude));
        }
    }

    // Token: 0x060018B8 RID: 6328 RVA: 0x001C86A4 File Offset: 0x001C68A4
    public PlayerBubble(PlayerGraphics playerGraphics, float intensity, float stickiness, float extraSpeed, Color startColor)
    {
        this.playerGraphics = playerGraphics;
        this.stickiness = stickiness;
        this.pos = Vector2.Lerp(playerGraphics.player.mainBodyChunk.pos, playerGraphics.tail[2].pos, UnityEngine.Random.value) + Custom.DegToVec(UnityEngine.Random.value * 360f) * (5f * UnityEngine.Random.value);
        this.originPoint = Custom.RotateAroundOrigo(this.pos - playerGraphics.tail[2].pos, -this.HeadRotation(playerGraphics, 1f));
        this.lastPos = this.pos;
        this.life = 1f;
        this.lifeTime = 10 + UnityEngine.Random.Range(0, UnityEngine.Random.Range(0, UnityEngine.Random.Range(0, UnityEngine.Random.Range(0, 200))));
        this.vel = playerGraphics.head.vel + Custom.DegToVec(UnityEngine.Random.value * 360f) * (UnityEngine.Random.value * UnityEngine.Random.value * 12f * Mathf.Pow(intensity, 1.5f) + extraSpeed);
        this.hollowNess = Mathf.Lerp(0.5f, 1.5f, UnityEngine.Random.value);
        if (this.Stickiness == 0f)
        {
            this.freeFloating = true;
        }
        else
        {
            this.stuckToOrigin = true;
        }
        this.liberatedOrigin = this.pos;
        this.lastLiberatedOrigin = this.pos;

        this.startColor = startColor;
    }

    public float HeadRotation(PlayerGraphics playerGraphics, float timeStacker)
    {
        float num = Custom.AimFromOneVectorToAnother(Vector2.Lerp(playerGraphics.drawPositions[0, 1], playerGraphics.drawPositions[0, 0], timeStacker), Vector2.Lerp(playerGraphics.head.lastPos, playerGraphics.head.pos, timeStacker));
        return num ;
    }

    // Token: 0x060018B9 RID: 6329 RVA: 0x001C8868 File Offset: 0x001C6A68
    public override void Update(bool eu)
    {
        this.vel = Vector3.Slerp(this.vel, Custom.DegToVec(UnityEngine.Random.value * 360f), 0.2f);
        this.lastLife = this.life;
        this.life -= 1f / (float)this.lifeTime;
        if (this.life <= 0f)
        {
            this.Destroy();
        }
        if (this.freeFloating && UnityEngine.Random.value < 0.5f)
        {
            this.hollow = (UnityEngine.Random.value < 0.5f);
        }
        if (this.room.GetTile(this.pos).Terrain == Room.Tile.TerrainType.Solid)
        {
            this.lifeTime = Math.Min(1, this.lifeTime - 5);
        }
        bool flag;
        if (ModManager.DLCShared)
        {
            flag = this.room.PointSubmerged(this.pos);
        }
        else
        {
            flag = (this.pos.y < this.room.FloatWaterLevel(this.pos));
        }
        if (flag)
        {
            this.vel *= 0.9f;
            this.vel.y += 4f;
        }
        if (this.stuckToOrigin)
        {
            Vector2 vector = this.playerGraphics.tail[2].pos + Custom.RotateAroundOrigo(this.originPoint, this.HeadRotation(playerGraphics, 1f));
            this.liberatedOriginVel = vector - this.liberatedOrigin;
            this.liberatedOrigin = vector;
            this.lastLiberatedOrigin = vector;
            if (this.life < 0.5f || UnityEngine.Random.value < 1f / (10f + this.Stickiness * 80f) || !Custom.DistLess(this.pos, vector, 10f + 90f * this.Stickiness))
            {
                this.stuckToOrigin = false;
            }
        }
        else if (!this.freeFloating)
        {
            this.lastLiberatedOrigin = this.liberatedOrigin;
            this.liberatedOriginVel = Vector2.Lerp(this.liberatedOriginVel, Custom.DirVec(this.liberatedOrigin, this.pos) * Mathf.Lerp(Vector2.Distance(this.liberatedOrigin, this.pos), 10f, 0.5f), 0.7f);
            this.liberatedOrigin += this.liberatedOriginVel;
            if (Custom.DistLess(this.liberatedOrigin, this.pos, 5f))
            {
                this.vel = Vector2.Lerp(this.vel, this.liberatedOriginVel, 0.3f);
                this.lifeTimeWhenFree = this.life;
                this.freeFloating = true;
            }
        }
        base.Update(eu);
    }

    // Token: 0x060018BA RID: 6330 RVA: 0x001C8B81 File Offset: 0x001C6D81
    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];
        sLeaser.sprites[0] = new FSprite("LizardBubble0", true);
        this.AddToContainer(sLeaser, rCam, null);
    }

    // Token: 0x060018BB RID: 6331 RVA: 0x001C8BAC File Offset: 0x001C6DAC
    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        float num = 0.625f * Mathf.Lerp(Mathf.Lerp(Mathf.Sin(3.1415927f * this.lastLife), this.lastLife, 0.5f), Mathf.Lerp(Mathf.Sin(3.1415927f * this.life), this.life, 0.5f), timeStacker);
        sLeaser.sprites[0].x = Mathf.Lerp(this.lastPos.x, this.pos.x, timeStacker) - camPos.x;
        sLeaser.sprites[0].y = Mathf.Lerp(this.lastPos.y, this.pos.y, timeStacker) - camPos.y;
        sLeaser.sprites[0].color = Color.Lerp(this.startColor, rCam.currentPalette.blackColor, 1f - Mathf.Clamp(Mathf.Lerp(this.lastLife, this.life, timeStacker) * 2f, 0f, 1f));
        int num2 = 0;
        if (this.hollow)
        {
            num2 = Custom.IntClamp((int)(Mathf.Pow(Mathf.InverseLerp(this.lifeTimeWhenFree, 0f, this.life), this.hollowNess) * 7f), 1, 7);
        }
        sLeaser.sprites[0].element = Futile.atlasManager.GetElementWithName(this._PlayerBubbleName[num2]);
        if (this.stuckToOrigin || !this.freeFloating)
        {
            Vector2 vector;
            if (this.stuckToOrigin)
            {
                vector = Vector2.Lerp(this.playerGraphics.tail[2].lastPos, this.playerGraphics.tail[2].pos, timeStacker) + Custom.RotateAroundOrigo(this.originPoint, this.HeadRotation(playerGraphics, timeStacker));
            }
            else
            {
                vector = Vector2.Lerp(this.lastLiberatedOrigin, this.liberatedOrigin, timeStacker);
            }
            float num3 = Vector2.Distance(Vector2.Lerp(this.lastPos, this.pos, timeStacker), vector) / 16f;
            sLeaser.sprites[0].scaleX = Mathf.Min(num, num / Mathf.Lerp(num3, 1f, 0.35f));
            sLeaser.sprites[0].scaleY = Mathf.Max(num, num3 - 0.3125f);
            sLeaser.sprites[0].rotation = Custom.AimFromOneVectorToAnother(Vector2.Lerp(this.lastPos, this.pos, timeStacker), vector);
            sLeaser.sprites[0].anchorY = Mathf.InverseLerp(1.25f, 0.3125f, num3) * 0.5f;
        }
        else
        {
            sLeaser.sprites[0].scaleX = num;
            sLeaser.sprites[0].scaleY = num;
            sLeaser.sprites[0].rotation = 0f;
            sLeaser.sprites[0].anchorY = 0.5f;
        }
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
    }

    // Token: 0x060018BC RID: 6332 RVA: 0x001C8E84 File Offset: 0x001C7084
    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
    }

    // Token: 0x060018BD RID: 6333 RVA: 0x001C8E88 File Offset: 0x001C7088
    public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        newContatiner ??= rCam.ReturnFContainer("Midground");
        foreach (FSprite fsprite in sLeaser.sprites)
        {
            fsprite.RemoveFromContainer();
            newContatiner.AddChild(fsprite);
        }
    }

    // Token: 0x04001638 RID: 5688
    public PlayerGraphics playerGraphics;

    public Color startColor;

    // Token: 0x04001639 RID: 5689
    public float life;

    // Token: 0x0400163A RID: 5690
    private float lastLife;

    // Token: 0x0400163B RID: 5691
    public int lifeTime;

    // Token: 0x0400163C RID: 5692
    private float hollowNess;

    // Token: 0x0400163D RID: 5693
    public bool hollow;

    // Token: 0x0400163E RID: 5694
    public Vector2 originPoint;

    // Token: 0x0400163F RID: 5695
    public bool stuckToOrigin;

    // Token: 0x04001640 RID: 5696
    public bool freeFloating;

    // Token: 0x04001641 RID: 5697
    public Vector2 liberatedOrigin;

    // Token: 0x04001642 RID: 5698
    public Vector2 liberatedOriginVel;

    // Token: 0x04001643 RID: 5699
    public Vector2 lastLiberatedOrigin;

    // Token: 0x04001644 RID: 5700
    public float lifeTimeWhenFree;

    // Token: 0x04001645 RID: 5701
    public float stickiness;

    // Token: 0x04001646 RID: 5702
    private string[] _PlayerBubbleName = new string[]
    {
        "LizardBubble0",
        "LizardBubble1",
        "LizardBubble2",
        "LizardBubble3",
        "LizardBubble4",
        "LizardBubble5",
        "LizardBubble6",
        "LizardBubble7"
    };
}

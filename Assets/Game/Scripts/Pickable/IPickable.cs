using UnityEngine;

public interface IPickable
{
    public bool RotatingToCamera { get; }
    public ItemData ItemData { get; }
    public void Pick(Transform followTarget);
    public void Drop();
    public void RotateToCamera();
    public void RotateVertical(float delta);
    public void RotateHorizontal(float delta);
}

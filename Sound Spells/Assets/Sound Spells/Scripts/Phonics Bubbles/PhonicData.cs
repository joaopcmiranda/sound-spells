using UnityEngine;

[CreateAssetMenu(fileName = "New Phonic", menuName = "Phonics/Phonic Data")]
public class PhonicData : ScriptableObject
{
    [Header("Phonic Information")]
    [Tooltip("The letter or phonic sound to display in the bubble, e.g., 'A', 'sh', 'ch'")]
    public string phonicText;

    [Tooltip("The full word the child needs to say to pop the bubble, e.g., 'Apple'")]
    public string targetWord;

    [Tooltip("The picture that represents the word, e.g., a sprite of an apple")]
    public Sprite displayImage;
}
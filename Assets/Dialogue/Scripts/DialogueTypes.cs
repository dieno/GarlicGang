using System;
namespace CommonCore.Dialogue
{
    internal enum FrameType
    {
        ChoiceFrame, TextFrame
    }

    internal class ChoiceNode
    {
        public readonly string Text;
        public readonly string Next;

        public ChoiceNode(string next, string text)
        {
            Text = text;
            Next = next;
        }
    }

    internal class Frame
    {
        public readonly string Background;
        public readonly string Image;
        public readonly string Next;
        public readonly string Music;
        public readonly string NameText;
        public readonly string Text;

        public Frame(string background, string image, string next, string music, string nameText, string text)
        {
            Background = background;
            Image = image;
            Next = next;
            Music = music;
            NameText = nameText;
            Text = text;
        }
    }

    internal class TextFrame : Frame
    {
        public TextFrame(string background, string image, string next, string music, string nameText, string text)
            : base(background, image, next, music, nameText, text)
        {
            
        }
    }

    internal class ChoiceFrame: Frame
    {
        public readonly ChoiceNode[] Choices;

        public ChoiceFrame(string background, string image, string next, string music, string nameText, string text, ChoiceNode[] choices)
            : base(background, image, next, music, nameText, text)
        {
            Choices = (ChoiceNode[])choices.Clone();
        }
    }

}
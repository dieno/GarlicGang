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

        public readonly Condition ShowCondition;
        public readonly Condition HideCondition;

        public ChoiceNode(string next, string text)
        {
            Text = text;
            Next = next;
        }

        public ChoiceNode(string next, string text, Condition showCondition, Condition hideCondition)
            : this(next, text)
        {
            ShowCondition = showCondition;
            HideCondition = hideCondition;
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

    internal class ConditionNode
    {
        public readonly string Next;
        public readonly Condition[] Conditions;

        public ConditionNode(string next, Condition[] conditions)
        {
            Next = next;
            Conditions = conditions;
        }
    }

    internal enum ConditionType
    {
        Flag, NoFlag, Item, Variable, Affinity, Quest //Eval is obviously not supported, might be able to provide Emit instead
    }

    internal enum ConditionOption
    {
        Consume, Greater, Less, Equal, GreaterEqual, LessEqual, Started, Finished
    }

    internal class Condition
    {
        public readonly ConditionType Type;
        public readonly string Target;
        public readonly ConditionOption? Option;
        public readonly int OptionValue;

        public Condition(ConditionType type, string target, ConditionOption? option, int optionValue)
        {
            Type = type;
            Target = target;
            Option = option;
            OptionValue = optionValue;
        }

        public bool Evaluate()
        {
            switch (Type)
            {
                case ConditionType.Flag:
                    return GameState.Instance.CampaignFlags.Contains(Target);
                case ConditionType.NoFlag:
                    return !GameState.Instance.CampaignFlags.Contains(Target);
                case ConditionType.Item:
                    int index = GameState.Instance.PlayerItems.IndexOf(Target);
                    if (index < 0)
                        return false;
                    else
                    {
                        if (Option.HasValue && Option.Value == ConditionOption.Consume && Convert.ToBoolean(OptionValue))
                            GameState.Instance.PlayerItems.RemoveAt(index);
                        return true;
                    }
                case ConditionType.Variable:
                    if (GameState.Instance.CampaignVars.ContainsKey(Target))
                        return EvaluateValueWithOption(GameState.Instance.CampaignVars[Target]);
                    else return false;                    
                case ConditionType.Affinity:
                    throw new NotImplementedException(); //could be supported, but isn't yet
                case ConditionType.Quest:
                    if (GameState.Instance.CampaignQuests.ContainsKey(Target))
                        return EvaluateValueWithOption(GameState.Instance.CampaignQuests[Target]);
                    else return false;
                default:
                    throw new NotSupportedException();
            }
        }

        private bool EvaluateValueWithOption(int value)
        {
            //technically out of spec but should be fine
            //probably the only instance that will work here but not with Katana
            switch (Option.Value)
            {
                case ConditionOption.Greater:
                    return value > OptionValue;
                case ConditionOption.Less:
                    return value < OptionValue;
                case ConditionOption.Equal:
                    return value == OptionValue;
                case ConditionOption.GreaterEqual:
                    return value >= OptionValue;
                case ConditionOption.LessEqual:
                    return value <= OptionValue;
                case ConditionOption.Started:
                    return value > 0;
                case ConditionOption.Finished:
                    return value < 0;
                default:
                    throw new NotSupportedException();
            }
        }
    }

}
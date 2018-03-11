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
        public readonly MicroscriptNode[] NextMicroscript;
        public readonly ConditionNode[] NextConditional;

        public readonly Condition ShowCondition;
        public readonly Condition HideCondition;

        public ChoiceNode(string next, string text)
        {
            Text = text;
            Next = next;
        }

        public ChoiceNode(string next, string text, Condition showCondition, Condition hideCondition, MicroscriptNode[] nextMicroscript, ConditionNode[] nextConditional)
            : this(next, text)
        {
            ShowCondition = showCondition;
            HideCondition = hideCondition;
            NextMicroscript = nextMicroscript;
            NextConditional = nextConditional;
        }

        public string EvaluateConditional()
        {
            for (int i = NextConditional.Length - 1; i >= 0; i--)
            {
                var nc = NextConditional[i];
                if (nc.Evaluate())
                    return nc.Next;
            }
            return null;
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
        public readonly ConditionNode[] NextConditional;
        public readonly MicroscriptNode[] NextMicroscript;

        public Frame(string background, string image, string next, string music, string nameText, string text, ConditionNode[] nextConditional, MicroscriptNode[] nextMicroscript)
        {
            Background = background;
            Image = image;
            Next = next;
            Music = music;
            NameText = nameText;
            Text = text;

            if (nextConditional != null && nextConditional.Length > 0)
                NextConditional = (ConditionNode[])nextConditional.Clone();

            if (nextMicroscript != null && nextMicroscript.Length > 0)
                NextMicroscript = (MicroscriptNode[])nextMicroscript.Clone();
        }

        public string EvaluateConditional()
        {
            for(int i = NextConditional.Length-1; i >= 0; i--)
            {
                var nc = NextConditional[i];
                if (nc.Evaluate())
                    return nc.Next;
            }
            return null;
        }
    }

    internal class TextFrame : Frame
    {
        public TextFrame(string background, string image, string next, string music, string nameText, string text, ConditionNode[] nextConditional, MicroscriptNode[] nextMicroscript)
            : base(background, image, next, music, nameText, text, nextConditional, nextMicroscript)
        {
            
        }
    }

    internal class ChoiceFrame: Frame
    {
        public readonly ChoiceNode[] Choices;

        public ChoiceFrame(string background, string image, string next, string music, string nameText, string text, ChoiceNode[] choices, ConditionNode[] nextConditional, MicroscriptNode[] nextMicroscript)
            : base(background, image, next, music, nameText, text, nextConditional, nextMicroscript)
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

        public bool Evaluate()
        {
            if (Conditions == null || Conditions.Length == 0) //odd, but in the spec
                return true;

            foreach(Condition c in Conditions)
            {
                if (!c.Evaluate())
                    return false;
            }
            return true;
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

    internal class MicroscriptNode
    {
        //TODO
    }

}
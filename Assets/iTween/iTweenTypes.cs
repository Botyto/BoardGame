public partial class iTween
{
    /// <summary>
    /// The type of easing to use based on Robert Penner's open source easing equations (http://www.robertpenner.com/easing_terms_of_use.html).
    /// </summary>
    public enum EaseType
    {
        Linear,
        Spring,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuint,
        EaseInSine,
        EaseOutSine,
        EaseInOutSine,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,
        EaseInCirc,
        EaseOutCirc,
        EaseInOutCirc,
        //bounce,
        EaseInBounce,
        EaseOutBounce,
        EaseInOutBounce,
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
        //elastic,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
        Punch,
    }

    /// <summary>
    /// The type of loop (if any) to use.  
    /// </summary>
    public enum LoopType
    {
        /// <summary>
        /// Do not loop.
        /// </summary>
        None,
        /// <summary>
        /// Rewind and replay.
        /// </summary>
        Loop,
        /// <summary>
        /// Ping pong the animation back and forth.
        /// </summary>
        PingPong
    }

    /// <summary>
    /// Many shaders use more than one color. Use can have iTween's Color methods operate on them by name.   
    /// </summary>
    public enum NamedValueColor
    {
        /// <summary>
        /// The main color of a material. Used by default and not required for Color methods to work in iTween.
        /// </summary>
        _Color,
        /// <summary>
        /// The specular color of a material (used in specular/glossy/vertexlit shaders).
        /// </summary>
        _SpecColor,
        /// <summary>
        /// The emissive color of a material (used in vertexlit shaders).
        /// </summary>
        _Emission,
        /// <summary>
        /// The reflection color of the material (used in reflective shaders).
        /// </summary>
        _ReflectColor
    }
}
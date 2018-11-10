using System;
using System.Collections;
using UnityEngine;

public partial class iTween
{
    /// <summary>
    /// Sets up a GameObject to avoid hiccups when an initial iTween is added. It's advisable to run this on every object you intend to run iTween on in its Start or Awake.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target to be initialized for iTween.
    /// </param>
    public static iTween Init(GameObject target)
    {
        return MoveBy(target, Vector3.zero, 0);
    }

    /// <summary>
    /// Returns a value to an 'oncallback' method interpolated between the supplied 'from' and 'to' values for application as desired.  Requires an 'onupdate' callback that accepts the same type as the supplied 'from' and 'to' properties.
    /// </summary>
    /// <param name="from">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> or <see cref="Vector3"/> or <see cref="Vector2"/> or <see cref="Color"/> or <see cref="Rect"/> for the starting value.
    /// </param> 
    /// <param name="to">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> or <see cref="Vector3"/> or <see cref="Vector2"/> or <see cref="Color"/> or <see cref="Rect"/> for the ending value.
    /// </param> 
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed (only works with Vector2, Vector3, and Floats)
    /// </param>	
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween ValueTo(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        if (!args.Contains("onupdate") || !args.Contains("from") || !args.Contains("to"))
        {
            Debug.LogError("iTween Error: ValueTo() requires an 'onupdate' callback function and a 'from' and 'to' property.  The supplied 'onupdate' callback must accept a single argument that is the same type as the supplied 'from' and 'to' properties!");
            return null;
        }
        else
        {
            //establish iTween:
            args["type"] = "value";

            if (args["from"].GetType() == typeof(Vector2))
            {
                args["method"] = "vector2";
            }
            else if (args["from"].GetType() == typeof(Vector3))
            {
                args["method"] = "vector3";
            }
            else if (args["from"].GetType() == typeof(Rect))
            {
                args["method"] = "rect";
            }
            else if (args["from"].GetType() == typeof(Single))
            {
                args["method"] = "float";
            }
            else if (args["from"].GetType() == typeof(Color))
            {
                args["method"] = "color";
            }
            else
            {
                Debug.LogError("iTween Error: ValueTo() only works with interpolating Vector3s, Vector2s, floats, ints, Rects and Colors!");
                return null;
            }

            //set a default easeType of linear if none is supplied since eased color interpolation is nearly unrecognizable:
            if (!args.Contains("easetype"))
            {
                args.Add("easetype", EaseType.Linear);
            }

            return Launch(target, args);
        }
    }

    /// <summary>
    /// Changes a GameObject's alpha value instantly then returns it to the provided alpha over time with MINIMUM customization options.  Identical to using ColorFrom and using the "a" parameter. 
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="alpha">
    /// A <see cref="System.Single"/> for the final alpha value of the animation.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween FadeFrom(GameObject target, float alpha, float time)
    {
        return FadeFrom(target, Hash("alpha", alpha, "time", time));
    }

    /// <summary>
    /// Changes a GameObject's alpha value instantly then returns it to the provided alpha over time with FULL customization options. Identical to using ColorFrom and using the "a" parameter.
    /// </summary>
    /// <param name="alpha">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the initial alpha value of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the initial alpha value of the animation.
    /// </param>
    /// <param name="includechildren">
    /// A <see cref="System.Boolean"/> for whether or not to include children of this GameObject. True by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween FadeFrom(GameObject target, Hashtable args)
    {
        return ColorFrom(target, args);
    }

    /// <summary>
    /// Changes a GameObject's alpha value over time with MINIMUM customization options. Identical to using ColorTo and using the "a" parameter.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="alpha">
    /// A <see cref="System.Single"/> for the final alpha value of the animation.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween FadeTo(GameObject target, float alpha, float time)
    {
        return FadeTo(target, Hash("alpha", alpha, "time", time));
    }

    /// <summary>
    /// Changes a GameObject's alpha value over time with FULL customization options. Identical to using ColorTo and using the "a" parameter.
    /// </summary>
    /// <param name="alpha">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the final alpha value of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the final alpha value of the animation.
    /// </param>
    /// <param name="includechildren">
    /// A <see cref="System.Boolean"/> for whether or not to include children of this GameObject. True by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween FadeTo(GameObject target, Hashtable args)
    {
        return ColorTo(target, args);
    }

    /// <summary>
    /// Changes a GameObject's color values instantly then returns them to the provided properties over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/> to change the GameObject's color to.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween ColorFrom(GameObject target, Color color, float time)
    {
        return ColorFrom(target, Hash("color", color, "time", time));
    }

    /// <summary>
    /// Changes a GameObject's color values instantly then returns them to the provided properties over time with FULL customization options.
    /// </summary>
    /// <param name="color">
    /// A <see cref="Color"/> to change the GameObject's color to.
    /// </param>
    /// <param name="r">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color red.
    /// </param>
    /// <param name="g">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color green.
    /// </param>
    /// <param name="b">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color green.
    /// </param>
    /// <param name="a">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the alpha.
    /// </param> 
    /// <param name="namedcolorvalue">
    /// A <see cref="NamedColorValue"/> or <see cref="System.String"/> for the individual setting of the alpha.
    /// </param> 
    /// <param name="includechildren">
    /// A <see cref="System.Boolean"/> for whether or not to include children of this GameObject. True by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween ColorFrom(GameObject target, Hashtable args)
    {
        Color fromColor = new Color();
        Color tempColor = new Color();

        //clean args:
        args = CleanArgs(args);

        //handle children:
        if (!args.Contains("includechildren") || (bool)args["includechildren"])
        {
            foreach (Transform child in target.transform)
            {
                Hashtable argsCopy = (Hashtable)args.Clone();
                argsCopy["ischild"] = true;
                ColorFrom(child.gameObject, argsCopy);
            }
        }

        //set a default easeType of linear if none is supplied since eased color interpolation is nearly unrecognizable:
        if (!args.Contains("easetype"))
        {
            args.Add("easetype", EaseType.Linear);
        }

        //set tempColor and base fromColor:
        if (target.GetComponent<Renderer>())
        {
            tempColor = fromColor = target.GetComponent<Renderer>().material.color;
        }
        else if (target.GetComponent<Light>())
        {
            tempColor = fromColor = target.GetComponent<Light>().color;
        }

        //set augmented fromColor:
        if (args.Contains("color"))
        {
            fromColor = (Color)args["color"];
        }
        else
        {
            if (args.Contains("r"))
            {
                fromColor.r = (float)args["r"];
            }
            if (args.Contains("g"))
            {
                fromColor.g = (float)args["g"];
            }
            if (args.Contains("b"))
            {
                fromColor.b = (float)args["b"];
            }
            if (args.Contains("a"))
            {
                fromColor.a = (float)args["a"];
            }
        }

        //alpha or amount?
        if (args.Contains("amount"))
        {
            fromColor.a = (float)args["amount"];
            args.Remove("amount");
        }
        else if (args.Contains("alpha"))
        {
            fromColor.a = (float)args["alpha"];
            args.Remove("alpha");
        }

        //apply fromColor:
        if (target.GetComponent<Renderer>())
        {
            target.GetComponent<Renderer>().material.color = fromColor;
        }
        else if (target.GetComponent<Light>())
        {
            target.GetComponent<Light>().color = fromColor;
        }

        //set new color arg:
        args["color"] = tempColor;

        //establish iTween:
        args["type"] = "color";
        args["method"] = "to";
        return Launch(target, args);
    }

    /// <summary>
    /// Changes a GameObject's color values over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/> to change the GameObject's color to.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween ColorTo(GameObject target, Color color, float time)
    {
        return ColorTo(target, Hash("color", color, "time", time));
    }

    /// <summary>
    /// Changes a GameObject's color values over time with FULL customization options.
    /// </summary>
    /// <param name="color">
    /// A <see cref="Color"/> to change the GameObject's color to.
    /// </param>
    /// <param name="r">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color red.
    /// </param>
    /// <param name="g">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color green.
    /// </param>
    /// <param name="b">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color green.
    /// </param>
    /// <param name="a">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the alpha.
    /// </param> 
    /// <param name="namedcolorvalue">
    /// A <see cref="NamedColorValue"/> or <see cref="System.String"/> for the individual setting of the alpha.
    /// </param> 
    /// <param name="includechildren">
    /// A <see cref="System.Boolean"/> for whether or not to include children of this GameObject. True by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween ColorTo(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //handle children:
        if (!args.Contains("includechildren") || (bool)args["includechildren"])
        {
            foreach (Transform child in target.transform)
            {
                Hashtable argsCopy = (Hashtable)args.Clone();
                argsCopy["ischild"] = true;
                ColorTo(child.gameObject, argsCopy);
            }
        }

        //set a default easeType of linear if none is supplied since eased color interpolation is nearly unrecognizable:
        if (!args.Contains("easetype"))
        {
            args.Add("easetype", EaseType.Linear);
        }

        //establish iTween:
        args["type"] = "color";
        args["method"] = "to";
        return Launch(target, args);
    }

    /// <summary>
    /// Instantly changes an AudioSource's volume and pitch then returns it to it's starting volume and pitch over time with MINIMUM customization options. Default AudioSource attached to GameObject will be used (if one exists) if not supplied.
    /// </summary>
    /// <param name="target"> 
    /// A <see cref="GameObject"/> to be the target of the animation which holds the AudioSource to be changed.
    /// </param>
    /// <param name="volume"> for the target level of volume.
    /// A <see cref="System.Single"/>
    /// </param>
    /// <param name="pitch"> for the target pitch.
    /// A <see cref="System.Single"/>
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween AudioFrom(GameObject target, float volume, float pitch, float time)
    {
        return AudioFrom(target, Hash("volume", volume, "pitch", pitch, "time", time));
    }

    /// <summary>
    /// Instantly changes an AudioSource's volume and pitch then returns it to it's starting volume and pitch over time with FULL customization options. Default AudioSource attached to GameObject will be used (if one exists) if not supplied. 
    /// </summary>
    /// <param name="audiosource">
    /// A <see cref="AudioSource"/> for which AudioSource to use.
    /// </param> 
    /// <param name="volume">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the target level of volume.
    /// </param>
    /// <param name="pitch">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the target pitch.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween AudioFrom(GameObject target, Hashtable args)
    {
        Vector2 tempAudioProperties;
        Vector2 fromAudioProperties;
        AudioSource tempAudioSource;

        //clean args:
        args = CleanArgs(args);

        //set tempAudioSource:
        if (args.Contains("audiosource"))
        {
            tempAudioSource = (AudioSource)args["audiosource"];
        }
        else
        {
            if (target.GetComponent<AudioSource>())
            {
                tempAudioSource = target.GetComponent<AudioSource>();
            }
            else
            {
                //throw error if no AudioSource is available:
                Debug.LogError("iTween Error: AudioFrom requires an AudioSource.");
                return null;
            }
        }

        //set tempAudioProperties:
        tempAudioProperties.x = fromAudioProperties.x = tempAudioSource.volume;
        tempAudioProperties.y = fromAudioProperties.y = tempAudioSource.pitch;

        //set augmented fromAudioProperties:
        if (args.Contains("volume"))
        {
            fromAudioProperties.x = (float)args["volume"];
        }
        if (args.Contains("pitch"))
        {
            fromAudioProperties.y = (float)args["pitch"];
        }

        //apply fromAudioProperties:
        tempAudioSource.volume = fromAudioProperties.x;
        tempAudioSource.pitch = fromAudioProperties.y;

        //set new volume and pitch args:
        args["volume"] = tempAudioProperties.x;
        args["pitch"] = tempAudioProperties.y;

        //set a default easeType of linear if none is supplied since eased audio interpolation is nearly unrecognizable:
        if (!args.Contains("easetype"))
        {
            args.Add("easetype", EaseType.Linear);
        }

        //establish iTween:
        args["type"] = "audio";
        args["method"] = "to";
        return Launch(target, args);
    }

    /// <summary>
    /// Fades volume and pitch of an AudioSource with MINIMUM customization options.  Default AudioSource attached to GameObject will be used (if one exists) if not supplied. 
    /// </summary>
    /// <param name="target"> 
    /// A <see cref="GameObject"/> to be the target of the animation which holds the AudioSource to be changed.
    /// </param>
    /// <param name="volume"> for the target level of volume.
    /// A <see cref="System.Single"/>
    /// </param>
    /// <param name="pitch"> for the target pitch.
    /// A <see cref="System.Single"/>
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween AudioTo(GameObject target, float volume, float pitch, float time)
    {
        return AudioTo(target, Hash("volume", volume, "pitch", pitch, "time", time));
    }

    /// <summary>
    /// Fades volume and pitch of an AudioSource with FULL customization options.  Default AudioSource attached to GameObject will be used (if one exists) if not supplied. 
    /// </summary>
    /// <param name="audiosource">
    /// A <see cref="AudioSource"/> for which AudioSource to use.
    /// </param> 
    /// <param name="volume">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the target level of volume.
    /// </param>
    /// <param name="pitch">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the target pitch.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween AudioTo(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //set a default easeType of linear if none is supplied since eased audio interpolation is nearly unrecognizable:
        if (!args.Contains("easetype"))
        {
            args.Add("easetype", EaseType.Linear);
        }

        //establish iTween:
        args["type"] = "audio";
        args["method"] = "to";
        return Launch(target, args);
    }

    /// <summary>
    /// Plays an AudioClip once based on supplied volume and pitch and following any delay with MINIMUM customization options. AudioSource is optional as iTween will provide one.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation which holds the AudioSource to be utilized.
    /// </param>
    /// <param name="audioclip">
    /// A <see cref="AudioClip"/> for a reference to the AudioClip to be played.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> for the time in seconds the action will wait before beginning.
    /// </param>
    public static iTween Stab(GameObject target, AudioClip audioclip, float delay)
    {
        return Stab(target, Hash("audioclip", audioclip, "delay", delay));
    }

    /// <summary>
    /// Plays an AudioClip once based on supplied volume and pitch and following any delay with FULL customization options. AudioSource is optional as iTween will provide one.
    /// </summary>
    /// <param name="audioclip">
    /// A <see cref="AudioClip"/> for a reference to the AudioClip to be played.
    /// </param> 
    /// <param name="audiosource">
    /// A <see cref="AudioSource"/> for which AudioSource to use
    /// </param> 
    /// <param name="volume">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the target level of volume.
    /// </param>
    /// <param name="pitch">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the target pitch.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the action will wait before beginning.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween Stab(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //establish iTween:
        args["type"] = "stab";
        return Launch(target, args);
    }

    /// <summary>
    /// Instantly rotates a GameObject to look at the supplied Vector3 then returns it to it's starting rotation over time with MINIMUM customization options. 
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> to be the Vector3 that the target will look towards.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween LookFrom(GameObject target, Vector3 looktarget, float time)
    {
        return LookFrom(target, Hash("looktarget", looktarget, "time", time));
    }

    /// <summary>
    /// Instantly rotates a GameObject to look at a supplied Transform or Vector3 then returns it to it's starting rotation over time with FULL customization options. 
    /// </summary>
    /// <param name="looktarget">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="axis">
    /// A <see cref="System.String"/>. Restricts rotation to the supplied axis only.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween LookFrom(GameObject target, Hashtable args)
    {
        Vector3 tempRotation;
        Vector3 tempRestriction;

        //clean args:
        args = CleanArgs(args);

        //set look:
        tempRotation = target.transform.eulerAngles;
        if (args["looktarget"].GetType() == typeof(Transform))
        {
            //target.transform.LookAt((Transform)args["looktarget"]);
            target.transform.LookAt((Transform)args["looktarget"], (Vector3?)args["up"] ?? Defaults.up);
        }
        else if (args["looktarget"].GetType() == typeof(Vector3))
        {
            //target.transform.LookAt((Vector3)args["looktarget"]);
            target.transform.LookAt((Vector3)args["looktarget"], (Vector3?)args["up"] ?? Defaults.up);
        }

        //axis restriction:
        if (args.Contains("axis"))
        {
            tempRestriction = target.transform.eulerAngles;
            switch ((string)args["axis"])
            {
                case "x":
                    tempRestriction.y = tempRotation.y;
                    tempRestriction.z = tempRotation.z;
                    break;
                case "y":
                    tempRestriction.x = tempRotation.x;
                    tempRestriction.z = tempRotation.z;
                    break;
                case "z":
                    tempRestriction.x = tempRotation.x;
                    tempRestriction.y = tempRotation.y;
                    break;
            }
            target.transform.eulerAngles = tempRestriction;
        }

        //set new rotation:
        args["rotation"] = tempRotation;

        //establish iTween
        args["type"] = "rotate";
        args["method"] = "to";
        return Launch(target, args);
    }

    /// <summary>
    /// Rotates a GameObject to look at the supplied Vector3 over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> to be the Vector3 that the target will look towards.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween LookTo(GameObject target, Vector3 looktarget, float time)
    {
        return LookTo(target, Hash("looktarget", looktarget, "time", time));
    }

    /// <summary>
    /// Rotates a GameObject to look at a supplied Transform or Vector3 over time with FULL customization options.
    /// </summary>
    /// <param name="looktarget">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="axis">
    /// A <see cref="System.String"/>. Restricts rotation to the supplied axis only.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween LookTo(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //additional property to ensure ConflictCheck can work correctly since Transforms are refrences:		
        if (args.Contains("looktarget"))
        {
            if (args["looktarget"].GetType() == typeof(Transform))
            {
                Transform transform = (Transform)args["looktarget"];
                args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }

        //establish iTween
        args["type"] = "look";
        args["method"] = "to";
        return Launch(target, args);
    }

    /// <summary>
    /// Changes a GameObject's position over time to a supplied destination with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="position">
    /// A <see cref="Vector3"/> for the destination Vector3.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween MoveTo(GameObject target, Vector3 position, float time)
    {
        return MoveTo(target, Hash("position", position, "time", time));
    }

    /// <summary>
    /// Changes a GameObject's position over time to a supplied destination with FULL customization options.
    /// </summary>
    /// <param name="position">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for a point in space the GameObject will animate to.
    /// </param>
    /// <param name="path">
    /// A <see cref="Transform[]"/> or <see cref="Vector3[]"/> for a list of points to draw a Catmull-Rom through for a curved animation path.
    /// </param>
    /// <param name="movetopath">
    /// A <see cref="System.Boolean"/> for whether to automatically generate a curve from the GameObject's current position to the beginning of the path. True by default.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="orienttopath">
    /// A <see cref="System.Boolean"/> for whether or not the GameObject will orient to its direction of travel.  False by default.
    /// </param>
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> or A <see cref="Transform"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="looktime">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the object will take to look at either the "looktarget" or "orienttopath".
    /// </param>
    /// <param name="lookahead">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for how much of a percentage to look ahead on a path to influence how strict "orientopath" is.
    /// </param>
    /// <param name="axis">
    /// A <see cref="System.String"/>. Restricts rotation to the supplied axis only.
    /// </param>
    /// <param name="islocal">
    /// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween MoveTo(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //additional property to ensure ConflictCheck can work correctly since Transforms are refrences:		
        if (args.Contains("position"))
        {
            if (args["position"].GetType() == typeof(Transform))
            {
                Transform transform = (Transform)args["position"];
                args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                args["scale"] = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }

        //establish iTween:
        args["type"] = "move";
        args["method"] = "to";
        return Launch(target, args);
    }

    /// <summary>
    /// Instantly changes a GameObject's position to a supplied destination then returns it to it's starting position over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="position">
    /// A <see cref="Vector3"/> for the destination Vector3.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween MoveFrom(GameObject target, Vector3 position, float time)
    {
        return MoveFrom(target, Hash("position", position, "time", time));
    }

    /// <summary>
    /// Instantly changes a GameObject's position to a supplied destination then returns it to it's starting position over time with FULL customization options.
    /// </summary>
    /// <param name="position">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for a point in space the GameObject will animate to.
    /// </param>
    /// <param name="path">
    /// A <see cref="Transform[]"/> or <see cref="Vector3[]"/> for a list of points to draw a Catmull-Rom through for a curved animation path.
    /// </param>
    /// <param name="movetopath">
    /// A <see cref="System.Boolean"/> for whether to automatically generate a curve from the GameObject's current position to the beginning of the path. True by default.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="orienttopath">
    /// A <see cref="System.Boolean"/> for whether or not the GameObject will orient to its direction of travel.  False by default.
    /// </param>
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> or A <see cref="Transform"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="looktime">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the object will take to look at either the "looktarget" or "orienttopath".
    /// </param>
    /// <param name="lookahead">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for how much of a percentage to look ahead on a path to influence how strict "orientopath" is.
    /// </param>
    /// <param name="islocal">
    /// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween MoveFrom(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        bool tempIsLocal;

        //set tempIsLocal:
        if (args.Contains("islocal"))
        {
            tempIsLocal = (bool)args["islocal"];
        }
        else
        {
            tempIsLocal = Defaults.isLocal;
        }

        if (args.Contains("path"))
        {
            Vector3[] fromPath;
            Vector3[] suppliedPath;
            if (args["path"].GetType() == typeof(Vector3[]))
            {
                Vector3[] temp = (Vector3[])args["path"];
                suppliedPath = new Vector3[temp.Length];
                Array.Copy(temp, suppliedPath, temp.Length);
            }
            else
            {
                Transform[] temp = (Transform[])args["path"];
                suppliedPath = new Vector3[temp.Length];
                for (int i = 0; i < temp.Length; i++)
                {
                    suppliedPath[i] = temp[i].position;
                }
            }
            if (suppliedPath[suppliedPath.Length - 1] != target.transform.position)
            {
                fromPath = new Vector3[suppliedPath.Length + 1];
                Array.Copy(suppliedPath, fromPath, suppliedPath.Length);
                if (tempIsLocal)
                {
                    fromPath[fromPath.Length - 1] = target.transform.localPosition;
                    target.transform.localPosition = fromPath[0];
                }
                else
                {
                    fromPath[fromPath.Length - 1] = target.transform.position;
                    target.transform.position = fromPath[0];
                }
                args["path"] = fromPath;
            }
            else
            {
                if (tempIsLocal)
                {
                    target.transform.localPosition = suppliedPath[0];
                }
                else
                {
                    target.transform.position = suppliedPath[0];
                }
                args["path"] = suppliedPath;
            }
        }
        else
        {
            Vector3 tempPosition;
            Vector3 fromPosition;

            //set tempPosition and base fromPosition:
            if (tempIsLocal)
            {
                tempPosition = fromPosition = target.transform.localPosition;
            }
            else
            {
                tempPosition = fromPosition = target.transform.position;
            }

            //set augmented fromPosition:
            if (args.Contains("position"))
            {
                if (args["position"].GetType() == typeof(Transform))
                {
                    Transform trans = (Transform)args["position"];
                    fromPosition = trans.position;
                }
                else if (args["position"].GetType() == typeof(Vector3))
                {
                    fromPosition = (Vector3)args["position"];
                }
            }
            else
            {
                if (args.Contains("x"))
                {
                    fromPosition.x = (float)args["x"];
                }
                if (args.Contains("y"))
                {
                    fromPosition.y = (float)args["y"];
                }
                if (args.Contains("z"))
                {
                    fromPosition.z = (float)args["z"];
                }
            }

            //apply fromPosition:
            if (tempIsLocal)
            {
                target.transform.localPosition = fromPosition;
            }
            else
            {
                target.transform.position = fromPosition;
            }

            //set new position arg:
            args["position"] = tempPosition;
        }

        //establish iTween:
        args["type"] = "move";
        args["method"] = "to";
        return Launch(target, args);
    }

    /// <summary>
    /// Translates a GameObject's position over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount of change in position to move the GameObject.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween MoveAdd(GameObject target, Vector3 amount, float time)
    {
        return MoveAdd(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Translates a GameObject's position over time with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount of change in position to move the GameObject.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="orienttopath">
    /// A <see cref="System.Boolean"/> for whether or not the GameObject will orient to its direction of travel.  False by default.
    /// </param>
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> or A <see cref="Transform"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="looktime">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the object will take to look at either the "looktarget" or "orienttopath".
    /// </param>
    /// <param name="axis">
    /// A <see cref="System.String"/>. Restricts rotation to the supplied axis only.
    /// </param>
    /// <param name="space">
    /// A <see cref="Space"/> or <see cref="System.String"/> for applying the transformation in either the world coordinate or local cordinate system. Defaults to local space.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween MoveAdd(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //establish iTween:
        args["type"] = "move";
        args["method"] = "add";
        return Launch(target, args);
    }

    /// <summary>
    /// Adds the supplied coordinates to a GameObject's postion with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount of change in position to move the GameObject.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween MoveBy(GameObject target, Vector3 amount, float time)
    {
        return MoveBy(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Adds the supplied coordinates to a GameObject's position with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount of change in position to move the GameObject.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="orienttopath">
    /// A <see cref="System.Boolean"/> for whether or not the GameObject will orient to its direction of travel.  False by default.
    /// </param>
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> or A <see cref="Transform"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="looktime">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the object will take to look at either the "looktarget" or "orienttopath".
    /// </param>
    /// <param name="axis">
    /// A <see cref="System.String"/>. Restricts rotation to the supplied axis only.
    /// </param>
    /// <param name="space">
    /// A <see cref="Space"/> or <see cref="System.String"/> for applying the transformation in either the world coordinate or local cordinate system. Defaults to local space.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween MoveBy(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //establish iTween:
        args["type"] = "move";
        args["method"] = "by";
        return Launch(target, args);
    }

    /// <summary>
    /// Changes a GameObject's scale over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="scale">
    /// A <see cref="Vector3"/> for the final scale.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween ScaleTo(GameObject target, Vector3 scale, float time)
    {
        return ScaleTo(target, Hash("scale", scale, "time", time));
    }

    /// <summary>
    /// Changes a GameObject's scale over time with FULL customization options.
    /// </summary>
    /// <param name="scale">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for the final scale.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween ScaleTo(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //additional property to ensure ConflictCheck can work correctly since Transforms are refrences:		
        if (args.Contains("scale"))
        {
            if (args["scale"].GetType() == typeof(Transform))
            {
                Transform transform = (Transform)args["scale"];
                args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                args["scale"] = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }

        //establish iTween:
        args["type"] = "scale";
        args["method"] = "to";
        return Launch(target, args);
    }

    /// <summary>
    /// Instantly changes a GameObject's scale then returns it to it's starting scale over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="scale">
    /// A <see cref="Vector3"/> for the final scale.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween ScaleFrom(GameObject target, Vector3 scale, float time)
    {
        return ScaleFrom(target, Hash("scale", scale, "time", time));
    }

    /// <summary>
    /// Instantly changes a GameObject's scale then returns it to it's starting scale over time with FULL customization options.
    /// </summary>
    /// <param name="scale">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for the final scale.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween ScaleFrom(GameObject target, Hashtable args)
    {
        Vector3 tempScale;
        Vector3 fromScale;

        //clean args:
        args = CleanArgs(args);

        //set base fromScale:
        tempScale = fromScale = target.transform.localScale;

        //set augmented fromScale:
        if (args.Contains("scale"))
        {
            if (args["scale"].GetType() == typeof(Transform))
            {
                Transform trans = (Transform)args["scale"];
                fromScale = trans.localScale;
            }
            else if (args["scale"].GetType() == typeof(Vector3))
            {
                fromScale = (Vector3)args["scale"];
            }
        }
        else
        {
            if (args.Contains("x"))
            {
                fromScale.x = (float)args["x"];
            }
            if (args.Contains("y"))
            {
                fromScale.y = (float)args["y"];
            }
            if (args.Contains("z"))
            {
                fromScale.z = (float)args["z"];
            }
        }

        //apply fromScale:
        target.transform.localScale = fromScale;

        //set new scale arg:
        args["scale"] = tempScale;

        //establish iTween:
        args["type"] = "scale";
        args["method"] = "to";
        return Launch(target, args);
    }

    /// <summary>
    /// Adds to a GameObject's scale over time with FULL customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount of scale to be added to the GameObject's current scale.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween ScaleAdd(GameObject target, Vector3 amount, float time)
    {
        return ScaleAdd(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Adds to a GameObject's scale over time with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount to be added to the GameObject's current scale.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween ScaleAdd(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //establish iTween:
        args["type"] = "scale";
        args["method"] = "add";
        return Launch(target, args);
    }

    /// <summary>
    /// Multiplies a GameObject's scale over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount of scale to be multiplied by the GameObject's current scale.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween ScaleBy(GameObject target, Vector3 amount, float time)
    {
        return ScaleBy(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Multiplies a GameObject's scale over time with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount to be multiplied to the GameObject's current scale.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween ScaleBy(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //establish iTween:
        args["type"] = "scale";
        args["method"] = "by";
        return Launch(target, args);
    }

    /// <summary>
    /// Rotates a GameObject to the supplied Euler angles in degrees over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="rotation">
    /// A <see cref="Vector3"/> for the target Euler angles in degrees to rotate to.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween RotateTo(GameObject target, Vector3 rotation, float time)
    {
        return RotateTo(target, Hash("rotation", rotation, "time", time));
    }

    /// <summary>
    /// Rotates a GameObject to the supplied Euler angles in degrees over time with FULL customization options.
    /// </summary>
    /// <param name="rotation">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for the target Euler angles in degrees to rotate to.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="islocal">
    /// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween RotateTo(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //additional property to ensure ConflictCheck can work correctly since Transforms are refrences:		
        if (args.Contains("rotation"))
        {
            if (args["rotation"].GetType() == typeof(Quaternion))
            {
                args["rotation"] = ((Quaternion)args["rotation"]).eulerAngles;
            }
            else if (args["rotation"].GetType() == typeof(Transform))
            {
                Transform transform = (Transform)args["rotation"];
                args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                args["scale"] = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }

        //establish iTween
        args["type"] = "rotate";
        args["method"] = "to";
        return Launch(target, args);
    }

    /// <summary>
    /// Instantly changes a GameObject's Euler angles in degrees then returns it to it's starting rotation over time (if allowed) with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="rotation">
    /// A <see cref="Vector3"/> for the target Euler angles in degrees to rotate from.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween RotateFrom(GameObject target, Vector3 rotation, float time)
    {
        return RotateFrom(target, Hash("rotation", rotation, "time", time));
    }

    /// <summary>
    /// Instantly changes a GameObject's Euler angles in degrees then returns it to it's starting rotation over time (if allowed) with FULL customization options.
    /// </summary>
    /// <param name="rotation">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for the target Euler angles in degrees to rotate to.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="islocal">
    /// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween RotateFrom(GameObject target, Hashtable args)
    {
        Vector3 tempRotation;
        Vector3 fromRotation;
        bool tempIsLocal;

        //clean args:
        args = CleanArgs(args);

        //set tempIsLocal:
        if (args.Contains("islocal"))
        {
            tempIsLocal = (bool)args["islocal"];
        }
        else
        {
            tempIsLocal = Defaults.isLocal;
        }

        //set tempRotation and base fromRotation:
        if (tempIsLocal)
        {
            tempRotation = fromRotation = target.transform.localEulerAngles;
        }
        else
        {
            tempRotation = fromRotation = target.transform.eulerAngles;
        }

        //set augmented fromRotation:
        if (args.Contains("rotation"))
        {
            if (args["rotation"].GetType() == typeof(Quaternion))
            {
                args["rotation"] = ((Quaternion)args["rotation"]).eulerAngles;
            }

            if (args["rotation"].GetType() == typeof(Transform))
            {
                Transform trans = (Transform)args["rotation"];
                fromRotation = trans.eulerAngles;
            }
            else if (args["rotation"].GetType() == typeof(Vector3))
            {
                fromRotation = (Vector3)args["rotation"];
            }
        }
        else
        {
            if (args.Contains("x"))
            {
                fromRotation.x = (float)args["x"];
            }
            if (args.Contains("y"))
            {
                fromRotation.y = (float)args["y"];
            }
            if (args.Contains("z"))
            {
                fromRotation.z = (float)args["z"];
            }
        }

        //apply fromRotation:
        if (tempIsLocal)
        {
            target.transform.localEulerAngles = fromRotation;
        }
        else
        {
            target.transform.eulerAngles = fromRotation;
        }

        //set new rotation arg:
        args["rotation"] = tempRotation;

        //establish iTween:
        args["type"] = "rotate";
        args["method"] = "to";
        return Launch(target, args);
    }

    /// <summary>
    /// Adds supplied Euler angles in degrees to a GameObject's rotation over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount of Euler angles in degrees to add to the current rotation of the GameObject.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween RotateAdd(GameObject target, Vector3 amount, float time)
    {
        return RotateAdd(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Adds supplied Euler angles in degrees to a GameObject's rotation over time with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount of Euler angles in degrees to add to the current rotation of the GameObject.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="space">
    /// A <see cref="Space"/> or <see cref="System.String"/> for applying the transformation in either the world coordinate or local cordinate system. Defaults to local space.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween RotateAdd(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //establish iTween:
        args["type"] = "rotate";
        args["method"] = "add";
        return Launch(target, args);
    }

    /// <summary>
    /// Multiplies supplied values by 360 and rotates a GameObject by calculated amount over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount to be multiplied by 360 to rotate the GameObject.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween RotateBy(GameObject target, Vector3 amount, float time)
    {
        return RotateBy(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Multiplies supplied values by 360 and rotates a GameObject by calculated amount over time with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount to be multiplied by 360 to rotate the GameObject.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="space">
    /// A <see cref="Space"/> or <see cref="System.String"/> for applying the transformation in either the world coordinate or local cordinate system. Defaults to local space.
    /// </param>
    /// <param name="islocal">
    /// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween RotateBy(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //establish iTween
        args["type"] = "rotate";
        args["method"] = "by";
        return Launch(target, args);
    }

    /// <summary>
    /// Randomly shakes a GameObject's position by a diminishing amount over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of shake.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween ShakePosition(GameObject target, Vector3 amount, float time)
    {
        return ShakePosition(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Randomly shakes a GameObject's position by a diminishing amount over time with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of shake.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x magnitude.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y magnitude.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z magnitude.
    /// </param>
    /// <param name="islocal">
    /// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
    /// </param>
    /// <param name="orienttopath">
    /// A <see cref="System.Boolean"/> for whether or not the GameObject will orient to its direction of travel.  False by default.
    /// </param>
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> or A <see cref="Transform"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="looktime">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the object will take to look at either the "looktarget" or "orienttopath".
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>  
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed. (only "loop" is allowed with shakes)
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween ShakePosition(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //establish iTween
        args["type"] = "shake";
        args["method"] = "position";
        return Launch(target, args);
    }

    /// <summary>
    /// Randomly shakes a GameObject's scale by a diminishing amount over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of shake.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween ShakeScale(GameObject target, Vector3 amount, float time)
    {
        return ShakeScale(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Randomly shakes a GameObject's scale by a diminishing amount over time with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of shake.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x magnitude.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y magnitude.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z magnitude.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed. (only "loop" is allowed with shakes)
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween ShakeScale(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //establish iTween
        args["type"] = "shake";
        args["method"] = "scale";
        return Launch(target, args);
    }

    /// <summary>
    /// Randomly shakes a GameObject's rotation by a diminishing amount over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of shake.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween ShakeRotation(GameObject target, Vector3 amount, float time)
    {
        return ShakeRotation(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Randomly shakes a GameObject's rotation by a diminishing amount over time with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of shake.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x magnitude.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y magnitude.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z magnitude.
    /// </param>
    /// <param name="space">
    /// A <see cref="Space"/> for applying the transformation in either the world coordinate or local cordinate system. Defaults to local space.
    /// </param> 
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed. (only "loop" is allowed with shakes)
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween ShakeRotation(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //establish iTween
        args["type"] = "shake";
        args["method"] = "rotation";
        return Launch(target, args);
    }

    /// <summary>
    /// Applies a jolt of force to a GameObject's position and wobbles it back to its initial position with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of the punch.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween PunchPosition(GameObject target, Vector3 amount, float time)
    {
        return PunchPosition(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Applies a jolt of force to a GameObject's position and wobbles it back to its initial position with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of shake.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x magnitude.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y magnitude.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z magnitude.
    /// </param>
    /// <param name="space">
    /// A <see cref="Space"/> for applying the transformation in either the world coordinate or local cordinate system. Defaults to local space.
    /// </param> 
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> or A <see cref="Transform"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="looktime">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the object will take to look at either the "looktarget".
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed. (only "loop" is allowed with punches)
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween PunchPosition(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //establish iTween
        args["type"] = "punch";
        args["method"] = "position";
        args["easetype"] = EaseType.Punch;
        return Launch(target, args);
    }

    /// <summary>
    /// Applies a jolt of force to a GameObject's rotation and wobbles it back to its initial rotation with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of the punch.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween PunchRotation(GameObject target, Vector3 amount, float time)
    {
        return PunchRotation(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Applies a jolt of force to a GameObject's rotation and wobbles it back to its initial rotation with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of shake.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x magnitude.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y magnitude.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z magnitude.
    /// </param>
    /// <param name="space">
    /// A <see cref="Space"/> for applying the transformation in either the world coordinate or local cordinate system. Defaults to local space.
    /// </param> 
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param> 
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed. (only "loop" is allowed with punches)
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween PunchRotation(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //establish iTween
        args["type"] = "punch";
        args["method"] = "rotation";
        args["easetype"] = EaseType.Punch;
        return Launch(target, args);
    }

    /// <summary>
    /// Applies a jolt of force to a GameObject's scale and wobbles it back to its initial scale with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of the punch.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static iTween PunchScale(GameObject target, Vector3 amount, float time)
    {
        return PunchScale(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Applies a jolt of force to a GameObject's scale and wobbles it back to its initial scale with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of shake.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x magnitude.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y magnitude.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z magnitude.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param> 
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed. (only "loop" is allowed with punches)
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static iTween PunchScale(GameObject target, Hashtable args)
    {
        //clean args:
        args = CleanArgs(args);

        //establish iTween
        args["type"] = "punch";
        args["method"] = "scale";
        args["easetype"] = EaseType.Punch;
        return Launch(target, args);
    }
}

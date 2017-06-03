
using UnitedSolution;using UnityEngine;

public static class ParticleExtensions
{
    /// Add Extension to the native ParticleSystem class.
    /// eg:  myParticleSystem.Scale(2); 
    public static void Scale(this ParticleSystem particles, float scale, bool includeChildren = true)
    {
        ParticleScaler.Scale(particles, scale, includeChildren);
    }

    public static void ScaleByTransform(this ParticleSystem particles, float scale, bool includeChildren = true)
    {
        ParticleScaler.ScaleByTransform(particles, scale, includeChildren);
    }
}

public static class ParticleScaler
{
    public static ParticleScalerOptions defaultOptions = new ParticleScalerOptions();

    static public void ScaleByTransform(ParticleSystem particles, float scale, bool includeChildren = true)
    {
        particles.scalingMode = ParticleSystemScalingMode.Local;
        particles.transform.localScale = particles.transform.localScale * scale;
        particles.gravityModifier *= scale;
        if (includeChildren)
        {
            var children = particles.GetComponentsInChildren<ParticleSystem>();
            for (var i = children.Length; i-- > 0;)
            {
                if (children[i] == particles) { continue; }
                children[i].scalingMode = ParticleSystemScalingMode.Local;
                children[i].transform.localScale = children[i].transform.localScale * scale;
                children[i].gravityModifier *= scale;
            }
        }
    }

    static public void Scale(ParticleSystem particles, float scale, bool includeChildren = true, ParticleScalerOptions options = null)
    {

        ScaleSystem(particles, scale, false, options);
        if (includeChildren)
        {
            var children = particles.GetComponentsInChildren<ParticleSystem>();
            for (var i = children.Length; i-- > 0;)
            {
                if (children[i] == particles) { continue; }
                ScaleSystem(children[i], scale, true, options);
            }
        }
    }

    private static void ScaleSystem(ParticleSystem particles, float scale, bool scalePosition, ParticleScalerOptions options = null)
    {
        if (options == null) { options = defaultOptions; }
        if (scalePosition) { particles.transform.localPosition *= scale; }

        particles.startSize *= scale;
        particles.gravityModifier *= scale;
        particles.startSpeed *= scale;

        if (options.shape)
        {
            var shape = particles.shape;
            shape.radius *= scale;
            shape.box = shape.box * scale;
        }

        /* Currently disabled due to a bug in Unity 5.3.4. 
        If any of these fields are using "Curves", the editor will shut down when they are modified.
        If you're not using any curves, feel free to uncomment the following lines;
        */
        if (options.velocity)
        {
            var vel = particles.velocityOverLifetime;
            vel.x = ScaleMinMaxCurve(vel.x, scale);
            vel.y = ScaleMinMaxCurve(vel.y, scale);
            vel.z = ScaleMinMaxCurve(vel.z, scale);
        }

        if (options.clampVelocity)
        {
            var clampVel = particles.limitVelocityOverLifetime;
            clampVel.limitX = ScaleMinMaxCurve(clampVel.limitX, scale);
            clampVel.limitY = ScaleMinMaxCurve(clampVel.limitY, scale);
            clampVel.limitZ = ScaleMinMaxCurve(clampVel.limitZ, scale);
        }

        if (options.force)
        {
            var force = particles.forceOverLifetime;
            force.x = ScaleMinMaxCurve(force.x, scale);
            force.y = ScaleMinMaxCurve(force.y, scale);
            force.z = ScaleMinMaxCurve(force.z, scale);
        }

    }

    private static ParticleSystem.MinMaxCurve ScaleMinMaxCurve(ParticleSystem.MinMaxCurve curve, float scale)
    {
        curve.curveMultiplier *= scale;
        curve.constantMin *= scale;
        curve.constantMax *= scale;
        ScaleCurve(curve.curveMin, scale);
        ScaleCurve(curve.curveMax, scale);
        return curve;
    }

    private static void ScaleCurve(AnimationCurve curve, float scale)
    {
        if (curve == null) { return; }
        for (int i = 0; i < curve.keys.Length; i++) { curve.keys[i].value *= scale; }
    }
}

public class ParticleScalerOptions
{
    public bool shape = true;
    public bool velocity = true;
    public bool clampVelocity = true;
    public bool force = true;
}
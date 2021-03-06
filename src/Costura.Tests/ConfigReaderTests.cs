﻿using System.Xml.Linq;
using NUnit.Framework;

[TestFixture]
public class ConfigReaderTests
{
    [Test]
    public void CanReadFalseNode()
    {
        var xElement = XElement.Parse(@"<Node attr='false'/>");
        Assert.IsFalse(Configuration.ReadBool(xElement, "attr", true));
    }

    [Test]
    public void CanReadTrueNode()
    {
        var xElement = XElement.Parse(@"<Node attr='true'/>");
        Assert.IsTrue(Configuration.ReadBool(xElement, "attr", false));
    }

    // These next 2 tests are because of https://github.com/Fody/Costura/issues/204

    [Test]
    public void TrimWhitespaceFromAttributeList()
    {
        var xElement = XElement.Parse(@"<Node attr=' Item'/>");
        var list = Configuration.ReadList(xElement, "attr");
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("Item", list[0]);
    }

    [Test]
    public void TrimWhitespaceFromElementList()
    {
        var xElement = XElement.Parse(@"<Node><attr>Item </attr></Node>");
        var list = Configuration.ReadList(xElement, "attr");
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("Item", list[0]);
    }

    [Test]
    public void DoesNotReadInvalidBoolNode()
    {
        var xElement = XElement.Parse(@"<Node attr='foo'/>");
        Assert.Throws<WeavingException>(() => Configuration.ReadBool(xElement, "attr", false), "Could not parse 'attr' from 'foo'.");
    }

    [Test]
    public void FalseIncludeDebugSymbols()
    {
        var xElement = XElement.Parse(@"<Costura IncludeDebugSymbols='false'/>");
        var config = new Configuration(xElement);
        Assert.IsFalse(config.IncludeDebugSymbols);
    }

    [Test]
    public void TrueDisableCompression()
    {
        var xElement = XElement.Parse(@"<Costura DisableCompression='true'/>");
        var config = new Configuration(xElement);
        Assert.IsTrue(config.DisableCompression);
    }

    [Test]
    public void TrueDisableCleanup()
    {
        var xElement = XElement.Parse(@"<Costura DisableCleanup='true'/>");
        var config = new Configuration(xElement);
        Assert.IsTrue(config.DisableCleanup);
    }

    [Test]
    public void FalseLoadAtModuleInit()
    {
        var xElement = XElement.Parse(@"<Costura LoadAtModuleInit='false'/>");
        var config = new Configuration(xElement);
        Assert.IsFalse(config.LoadAtModuleInit);
    }

    [Test]
    public void TrueCreateTemporaryAssemblies()
    {
        var xElement = XElement.Parse(@"<Costura CreateTemporaryAssemblies='true'/>");
        var config = new Configuration(xElement);
        Assert.IsTrue(config.CreateTemporaryAssemblies);
    }

    [Test]
    public void ExcludeAssembliesNode()
    {
        var xElement = XElement.Parse(@"
<Costura>
    <ExcludeAssemblies>
Foo
Bar
    </ExcludeAssemblies>
</Costura>");
        var config = new Configuration(xElement);
        Assert.AreEqual("Foo", config.ExcludeAssemblies[0]);
        Assert.AreEqual("Bar", config.ExcludeAssemblies[1]);
    }

    [Test]
    public void ExcludeAssembliesAttribute()
    {
        var xElement = XElement.Parse(@"
<Costura ExcludeAssemblies='Foo|Bar'/>");
        var config = new Configuration(xElement);
        Assert.AreEqual("Foo", config.ExcludeAssemblies[0]);
        Assert.AreEqual("Bar", config.ExcludeAssemblies[1]);
    }

    [Test]
    public void ExcludeAssembliesCombined()
    {
        var xElement = XElement.Parse(@"
<Costura  ExcludeAssemblies='Foo'>
    <ExcludeAssemblies>
Bar
    </ExcludeAssemblies>
</Costura>");
        var config = new Configuration(xElement);
        Assert.AreEqual("Foo", config.ExcludeAssemblies[0]);
        Assert.AreEqual("Bar", config.ExcludeAssemblies[1]);
    }

    [Test]
    public void IncludeAssembliesNode()
    {
        var xElement = XElement.Parse(@"
<Costura>
    <IncludeAssemblies>
Foo
Bar
    </IncludeAssemblies>
</Costura>");
        var config = new Configuration(xElement);
        Assert.AreEqual("Foo", config.IncludeAssemblies[0]);
        Assert.AreEqual("Bar", config.IncludeAssemblies[1]);
    }

    [Test]
    public void IncludeAssembliesAttribute()
    {
        var xElement = XElement.Parse(@"
<Costura IncludeAssemblies='Foo|Bar'/>");
        var config = new Configuration(xElement);
        Assert.AreEqual("Foo", config.IncludeAssemblies[0]);
        Assert.AreEqual("Bar", config.IncludeAssemblies[1]);
    }

    [Test]
    public void IncludeAndExcludeAssembliesAttribute()
    {
        var xElement = XElement.Parse(@"
<Costura IncludeAssemblies='Bar' ExcludeAssemblies='Foo'/>");
        Assert.Throws<WeavingException>(() => new Configuration(xElement), "Either configure IncludeAssemblies OR ExcludeAssemblies, not both.");
    }

    [Test]
    public void IncludeAssembliesCombined()
    {
        var xElement = XElement.Parse(@"
<Costura  IncludeAssemblies='Foo'>
    <IncludeAssemblies>
Bar
    </IncludeAssemblies>
</Costura>");
        var config = new Configuration(xElement);
        Assert.AreEqual("Foo", config.IncludeAssemblies[0]);
        Assert.AreEqual("Bar", config.IncludeAssemblies[1]);
    }

    [Test]
    public void Unmanaged32AssembliesNode()
    {
        var xElement = XElement.Parse(@"
<Costura>
    <Unmanaged32Assemblies>
Foo
Bar
    </Unmanaged32Assemblies>
</Costura>");
        var config = new Configuration(xElement);
        Assert.AreEqual("Foo", config.Unmanaged32Assemblies[0]);
        Assert.AreEqual("Bar", config.Unmanaged32Assemblies[1]);
    }

    [Test]
    public void Unmanaged32AssembliesAttribute()
    {
        var xElement = XElement.Parse(@"
<Costura Unmanaged32Assemblies='Foo|Bar'/>");
        var config = new Configuration(xElement);
        Assert.AreEqual("Foo", config.Unmanaged32Assemblies[0]);
        Assert.AreEqual("Bar", config.Unmanaged32Assemblies[1]);
    }

    [Test]
    public void Unmanaged32AssembliesCombined()
    {
        var xElement = XElement.Parse(@"
<Costura  Unmanaged32Assemblies='Foo'>
    <Unmanaged32Assemblies>
Bar
    </Unmanaged32Assemblies>
</Costura>");
        var config = new Configuration(xElement);
        Assert.AreEqual("Foo", config.Unmanaged32Assemblies[0]);
        Assert.AreEqual("Bar", config.Unmanaged32Assemblies[1]);
    }

    [Test]
    public void Unmanaged64AssembliesNode()
    {
        var xElement = XElement.Parse(@"
<Costura>
    <Unmanaged64Assemblies>
Foo
Bar
    </Unmanaged64Assemblies>
</Costura>");
        var config = new Configuration(xElement);
        Assert.AreEqual("Foo", config.Unmanaged64Assemblies[0]);
        Assert.AreEqual("Bar", config.Unmanaged64Assemblies[1]);
    }

    [Test]
    public void Unmanaged64AssembliesAttribute()
    {
        var xElement = XElement.Parse(@"
<Costura Unmanaged64Assemblies='Foo|Bar'/>");
        var config = new Configuration(xElement);
        Assert.AreEqual("Foo", config.Unmanaged64Assemblies[0]);
        Assert.AreEqual("Bar", config.Unmanaged64Assemblies[1]);
    }

    [Test]
    public void Unmanaged64AssembliesCombined()
    {
        var xElement = XElement.Parse(@"
<Costura  Unmanaged64Assemblies='Foo'>
    <Unmanaged64Assemblies>
Bar
    </Unmanaged64Assemblies>
</Costura>");
        var config = new Configuration(xElement);
        Assert.AreEqual("Foo", config.Unmanaged64Assemblies[0]);
        Assert.AreEqual("Bar", config.Unmanaged64Assemblies[1]);
    }

    [Test]
    public void PreloadOrderNode()
    {
        var xElement = XElement.Parse(@"
<Costura>
    <PreloadOrder>
Foo
Bar
    </PreloadOrder>
</Costura>");
        var config = new Configuration(xElement);
        Assert.AreEqual("Foo", config.PreloadOrder[0]);
        Assert.AreEqual("Bar", config.PreloadOrder[1]);
    }

    [Test]
    public void PreloadOrderAttribute()
    {
        var xElement = XElement.Parse(@"
<Costura PreloadOrder='Foo|Bar'/>");
        var config = new Configuration(xElement);
        Assert.AreEqual("Foo", config.PreloadOrder[0]);
        Assert.AreEqual("Bar", config.PreloadOrder[1]);
    }

    [Test]
    public void PreloadOrderCombined()
    {
        var xElement = XElement.Parse(@"
<Costura  PreloadOrder='Foo'>
    <PreloadOrder>
Bar
    </PreloadOrder>
</Costura>");
        var config = new Configuration(xElement);
        Assert.AreEqual("Foo", config.PreloadOrder[0]);
        Assert.AreEqual("Bar", config.PreloadOrder[1]);
    }
}
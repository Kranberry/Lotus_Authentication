using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Lotus_Authentication.API.ApiDocumentation;

[XmlRoot("doc")]
public class DocumentationModel
{
    [XmlArray("members")]
    [XmlArrayItem("member")]
    public Method[] Methods { get; set; }
}

public class Method
{
    [XmlElement("route")]
    public string Route { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlElement("method")]
    public string RestMethod { get; set; }

    [XmlElement("summary")]
    public string Summary { get; init; }

    [XmlElement("isActive")]
    public bool IsActive { get; init; }

    [XmlElement("returns")]
    public string Returns { get; init; }

    [XmlElement("exception")]
    public string[] Exceptions { get; init; }

    [XmlArray("header")]
    [XmlArrayItem("param")]
    public Parameter[] HeaderParameters { get; init; } = Array.Empty<Parameter>();

    [XmlArray("query")]
    [XmlArrayItem("param")]
    public Parameter[] QueryParameters { get; init; } = Array.Empty<Parameter>();

    [XmlArray("body")]
    [XmlArrayItem("param")]
    public Parameter[] BodyParameters { get; init; } = Array.Empty<Parameter>();

    [XmlArray("results")]
    [XmlArrayItem("result")]
    public Result[] Results { get;init; } = Array.Empty<Result>();

    private Regex NiceNameRegex = new(@"(?<=\.)[a-zA-Z]+(?=\()");
    private Regex ControllerNameRegex = new(@"(?<=API.Controllers.)[a-zA-Z]+(?=\.)");

    public string GetNiceName() => NiceNameRegex.Match(Name).Value;

    public string GetControllerName() => ControllerNameRegex.Match(Name).Value;
}

public class Parameter
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("sample")]
    public string Sample { get; set; }

    [XmlAttribute("required")]
    public bool Required { get; set; }

    [XmlText]
    public string Value { get; set; }
}

public class Result
{
    [XmlAttribute("status")]
    public int StatusCode { get; set; }

    [XmlAttribute("reason")]
    public string Reason { get; set; }

    [XmlText]
    public string Summary { get; set; }

    [XmlElement("param")]
    public Parameter[] Parameters { get; init; } = Array.Empty<Parameter>();

}
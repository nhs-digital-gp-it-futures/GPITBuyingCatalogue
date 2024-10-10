using System.Collections.Generic;
using System.Xml.Serialization;

namespace BuyingCatalogueFunction.OrganisationImport.Models;

[XmlRoot(ElementName="PublicationType")]
public class PublicationType {
	[XmlAttribute(AttributeName="value")]
	public string Value { get; set; }
}

[XmlRoot(ElementName="PublicationSource")]
public class PublicationSource {
	[XmlAttribute(AttributeName="value")]
	public string Value { get; set; }
}

[XmlRoot(ElementName="PublicationDate")]
public class PublicationDate {
	[XmlAttribute(AttributeName="value")]
	public string Value { get; set; }
}

[XmlRoot(ElementName="PublicationSeqNum")]
public class PublicationSeqNum {
	[XmlAttribute(AttributeName="value")]
	public string Value { get; set; }
}

[XmlRoot(ElementName="FileCreationDateTime")]
public class FileCreationDateTime {
	[XmlAttribute(AttributeName="value")]
	public string Value { get; set; }
}

[XmlRoot(ElementName="RecordCount")]
public class RecordCount {
	[XmlAttribute(AttributeName="value")]
	public string Value { get; set; }
}

[XmlRoot(ElementName="ContentDescription")]
public class ContentDescription {
	[XmlAttribute(AttributeName="value")]
	public string Value { get; set; }
}

[XmlRoot(ElementName="PrimaryRole")]
public class PrimaryRole {
	[XmlAttribute(AttributeName="id")]
	public string Id { get; set; }
	[XmlAttribute(AttributeName="displayName")]
	public string DisplayName { get; set; }
}

[XmlRoot(ElementName="PrimaryRoleScope")]
public class PrimaryRoleScope {
	[XmlElement(ElementName="PrimaryRole")]
	public List<PrimaryRole> PrimaryRole { get; set; }
}

[XmlRoot(ElementName="Manifest")]
public class Manifest {
	[XmlElement(ElementName="PublicationType")]
	public PublicationType PublicationType { get; set; }
	[XmlElement(ElementName="PublicationSource")]
	public PublicationSource PublicationSource { get; set; }
	[XmlElement(ElementName="PublicationDate")]
	public PublicationDate PublicationDate { get; set; }
	[XmlElement(ElementName="PublicationSeqNum")]
	public PublicationSeqNum PublicationSeqNum { get; set; }
	[XmlElement(ElementName="FileCreationDateTime")]
	public FileCreationDateTime FileCreationDateTime { get; set; }
	[XmlElement(ElementName="RecordCount")]
	public RecordCount RecordCount { get; set; }
	[XmlElement(ElementName="ContentDescription")]
	public ContentDescription ContentDescription { get; set; }
	[XmlElement(ElementName="PrimaryRoleScope")]
	public PrimaryRoleScope PrimaryRoleScope { get; set; }
}

[XmlRoot(ElementName="concept")]
public class Concept {
	[XmlAttribute(AttributeName="id")]
	public string Id { get; set; }
	[XmlAttribute(AttributeName="code")]
	public string Code { get; set; }
	[XmlAttribute(AttributeName="displayName")]
	public string DisplayName { get; set; }
}

[XmlRoot(ElementName="CodeSystem")]
public class CodeSystem {
	[XmlElement(ElementName="concept")]
	public List<Concept> Concept { get; set; }
	[XmlAttribute(AttributeName="name")]
	public string Name { get; set; }
	[XmlAttribute(AttributeName="oid")]
	public string Oid { get; set; }
}

[XmlRoot(ElementName="CodeSystems")]
public class CodeSystems {
	[XmlElement(ElementName="CodeSystem")]
	public List<CodeSystem> CodeSystem { get; set; }
}

[XmlRoot(ElementName="Type")]
public class Type {
	[XmlAttribute(AttributeName="value")]
	public string Value { get; set; }
}

[XmlRoot(ElementName="Start")]
public class Start {
	[XmlAttribute(AttributeName="value")]
	public string Value { get; set; }
}

[XmlRoot(ElementName="Date")]
public class Date {
	[XmlElement(ElementName="Type")]
	public Type Type { get; set; }
	[XmlElement(ElementName="Start")]
	public Start Start { get; set; }
}

[XmlRoot(ElementName="OrgId")]
public class OrgId {
	[XmlAttribute(AttributeName="root")]
	public string Root { get; set; }
	[XmlAttribute(AttributeName="assigningAuthorityName")]
	public string AssigningAuthorityName { get; set; }
	[XmlAttribute(AttributeName="extension")]
	public string Extension { get; set; }
}

[XmlRoot(ElementName="Status")]
public class Status {
	[XmlAttribute(AttributeName="value")]
	public string Value { get; set; }
}

[XmlRoot(ElementName="LastChangeDate")]
public class LastChangeDate {
	[XmlAttribute(AttributeName="value")]
	public string Value { get; set; }
}

[XmlRoot(ElementName="Location")]
public class Location {
	[XmlElement(ElementName="AddrLn1")]
    public string AddrLn1 { get; set; }
    [XmlElement(ElementName="AddrLn2")]
    public string AddrLn2 { get; set; }
    [XmlElement(ElementName="AddrLn3")]
    public string AddrLn3 { get; set; }
	[XmlElement(ElementName="Town")]
    public string Town { get; set; }
	[XmlElement(ElementName="County")]
    public string County { get; set; }
	[XmlElement(ElementName="PostCode")]
    public string PostCode { get; set; }
	[XmlElement(ElementName="Country")]
	public string Country { get; set; }
	[XmlElement(ElementName="UPRN")]
    public string UPRN { get; set; }
}

[XmlRoot(ElementName="GeoLoc")]
public class GeoLoc {
	[XmlElement(ElementName="Location")]
	public Location Location { get; set; }
}

[XmlRoot(ElementName="Role")]
public class Role {
	[XmlElement(ElementName="Date")]
	public Date Date { get; set; }
	[XmlElement(ElementName="Status")]
	public Status Status { get; set; }
	[XmlAttribute(AttributeName="id")]
	public string Id { get; set; }
	[XmlAttribute(AttributeName="uniqueRoleId")]
	public int UniqueRoleId { get; set; }
	[XmlAttribute(AttributeName="primaryRole")]
	public bool PrimaryRole { get; set; }
}

[XmlRoot(ElementName="Roles")]
public class RolesRoot {
	[XmlElement(ElementName="Role")]
	public List<Role> Roles { get; set; }
}

[XmlRoot(ElementName="PrimaryRoleId")]
public class PrimaryRoleId {
	[XmlAttribute(AttributeName="id")]
	public string Id { get; set; }
	[XmlAttribute(AttributeName="uniqueRoleId")]
	public string UniqueRoleId { get; set; }
}

[XmlRoot(ElementName="Target")]
public class Target {
	[XmlElement(ElementName="OrgId")]
	public OrgId OrgId { get; set; }
	[XmlElement(ElementName="PrimaryRoleId")]
	public PrimaryRoleId PrimaryRoleId { get; set; }
}

[XmlRoot(ElementName="Rel")]
public class Relationship {
	[XmlElement(ElementName="Date")]
	public Date Date { get; set; }
	[XmlElement(ElementName="Status")]
	public Status Status { get; set; }
	[XmlElement(ElementName="Target")]
	public Target Target { get; set; }
	[XmlAttribute(AttributeName="id")]
	public string Id { get; set; }
	[XmlAttribute(AttributeName="uniqueRelId")]
	public int UniqueRelId { get; set; }
}

[XmlRoot(ElementName="Rels")]
public class RelationshipsRoot {
	[XmlElement(ElementName="Rel")]
	public List<Relationship> Relationship { get; set; }
}

[XmlRoot(ElementName="Succ")]
public class Succ {
	[XmlElement(ElementName="Date")]
	public Date Date { get; set; }
	[XmlElement(ElementName="Type")]
	public string Type { get; set; }
	[XmlElement(ElementName="Target")]
	public Target Target { get; set; }
	[XmlAttribute(AttributeName="uniqueSuccId")]
	public string UniqueSuccId { get; set; }
}

[XmlRoot(ElementName="Succs")]
public class Succs {
	[XmlElement(ElementName="Succ")]
	public Succ Succ { get; set; }
}

[XmlRoot(ElementName="Organisation")]
public class Organisation {
	[XmlElement(ElementName="Name")]
	public string Name { get; set; }
	[XmlElement(ElementName="Date")]
	public Date Date { get; set; }
	[XmlElement(ElementName="OrgId")]
	public OrgId OrgId { get; set; }
	[XmlElement(ElementName="Status")]
	public Status Status { get; set; }
	[XmlElement(ElementName="LastChangeDate")]
	public LastChangeDate LastChangeDate { get; set; }
	[XmlElement(ElementName="GeoLoc")]
	public GeoLoc GeoLoc { get; set; }
	[XmlElement(ElementName="Roles")]
	public RolesRoot RolesRoot { get; set; }
	[XmlElement(ElementName="Rels")]
	public RelationshipsRoot RelationshipsRoot { get; set; }
	[XmlElement(ElementName="Succs")]
	public Succs Succs { get; set; }
	[XmlAttribute(AttributeName="orgRecordClass")]
	public string OrgRecordClass { get; set; }
}

[XmlRoot(ElementName="Organisations")]
public class OrganisationsRoot {
	[XmlElement(ElementName="Organisation")]
	public List<Organisation> Organisations { get; set; }
}

[XmlRoot(ElementName="OrgRefData", Namespace="http://refdata.hscic.gov.uk/org/v2-0-0")]
public class OrgRefData {
    [XmlElement(ElementName="Manifest", Namespace = "")]
	public Manifest Manifest { get; set; }

	[XmlElement(ElementName="CodeSystems", Namespace = "")]
	public CodeSystems CodeSystems { get; set; }

	[XmlElement(ElementName="Organisations", Namespace = "")]
	public OrganisationsRoot OrganisationsRoot { get; set; }

	[XmlAttribute(AttributeName="HSCOrgRefData", Namespace="http://www.w3.org/2000/xmlns/")]
	public string HSCOrgRefData { get; set; }

	[XmlAttribute(AttributeName="schemaLocation", Namespace="http://www.w3.org/2001/XMLSchema-instance")]
	public string SchemaLocation { get; set; }
}

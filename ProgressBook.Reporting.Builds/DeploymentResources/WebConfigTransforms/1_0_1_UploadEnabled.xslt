<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="@* | node()">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()"/>
        </xsl:copy>
    </xsl:template>

  <xsl:template match="/configuration/appSettings">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
      <xsl:if test="not(add[@key='UploadEnabled'])">
        <xsl:text>  </xsl:text>
        <xsl:call-template name="addAppSettingsKey">
          <xsl:with-param name="keyName" select="'UploadEnabled'"/>
          <xsl:with-param name="keyValue" select="'true'"/>
        </xsl:call-template>
        <xsl:text>&#13;&#10;</xsl:text>
      </xsl:if>
    </xsl:copy>
  </xsl:template>


  <xsl:template name="addAppSettingsKey">
    <xsl:param name="keyName"/>
    <xsl:param name="keyValue"/>
    <xsl:element name="add">
      <xsl:attribute name="key">
        <xsl:value-of select="$keyName"/>
      </xsl:attribute>
      <xsl:attribute name="value">
        <xsl:value-of select="$keyValue"/>
      </xsl:attribute>
    </xsl:element>
  </xsl:template>
</xsl:stylesheet>

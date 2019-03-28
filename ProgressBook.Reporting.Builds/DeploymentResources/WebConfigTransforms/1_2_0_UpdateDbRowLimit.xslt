<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:msasm="urn:schemas-microsoft-com:asm.v1"
                xmlns:safunc="http://software-answers.com/xslt/functions"
                exclude-result-prefixes="msxsl safunc msasm">

  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/configuration/appSettings/add[@key='DbRowLimit']">
    <add key="DbRowLimit" value="300000" />
  </xsl:template>

</xsl:stylesheet>

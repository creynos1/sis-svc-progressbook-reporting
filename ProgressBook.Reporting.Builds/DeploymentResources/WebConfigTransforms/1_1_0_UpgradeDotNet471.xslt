<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:msasm="urn:schemas-microsoft-com:asm.v1"
                xmlns:safunc="http://software-answers.com/xslt/functions"
                exclude-result-prefixes="msxsl safunc msasm">

  <xsl:output method="xml" indent="yes"/>

  <msxsl:script implements-prefix="safunc" language="C#">
    <![CDATA[
			public bool IsMatch(string pattern, string input)
			{
				var options = RegexOptions.IgnoreCase;
	
				return  System.Text.RegularExpressions.Regex.IsMatch(input, pattern, options);	
			}
		]]>
  </msxsl:script>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/configuration/system.web/compilation/@targetFramework[.='4.6.1']">
    <xsl:attribute name="targetFramework">
      <xsl:value-of select="'4.7.1'"/>
    </xsl:attribute>
  </xsl:template>

</xsl:stylesheet>

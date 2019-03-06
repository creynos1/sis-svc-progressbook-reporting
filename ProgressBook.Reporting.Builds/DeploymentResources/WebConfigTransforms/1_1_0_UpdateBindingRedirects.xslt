<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
				xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
				xmlns:msxsl="urn:schemas-microsoft-com:xslt"
				xmlns:msasm="urn:schemas-microsoft-com:asm.v1"
				xmlns:safunc="http://software-answers.com/xslt/functions"
				exclude-result-prefixes="msxsl msasm safunc">

  <xsl:output method="xml" indent="yes"/>

  <!-- 
		need to process:
		
		- when runtime element is missing all together
		- when assemblyIdentity element is missing or there is no assemblyBinding element
		- when assembly binding is there and the newVersion 'x.x.x.x' is "less than" the oldversion
		- when assembly binding is there and the newVersion 'x.x.x.x' is "greater than" the oldversion
	-->

  <!--	
		this is an xml fragment that contains the dependent assemblies to add/update 
		the format is as follows and match to the attributes from the dependentAssembly nodes:
		<assembly 
			name="[REQUIRED]" 
			publicKeyToken="[REQUIRED]" 
			newVersion="[REQUIRED] in x.x.x.x"
			culture="[optional]" 
			oldVersion="[optional, default is 0.0.0.0-@newVersion]
	-->
  <xsl:variable name="assemblies.tf">
    <assembly name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" newVersion="11.0.0.0" culture="neutral"/>
    <assembly name="System.IdentityModel.Tokens.Jwt" publicKeyToken="31bf3856ad364e35" newVersion="5.2.1.0" culture="neutral"/>
    <assembly name="System.Web.Http" publicKeyToken="31bf3856ad364e35" newVersion="5.2.4.0" culture="neutral"/>
    <assembly name="System.Net.Http.Formatting" publicKeyToken="31bf3856ad364e35" newVersion="5.2.4.0" culture="neutral"/>
    <assembly name="Microsoft.IdentityModel.Tokens" publicKeyToken="31bf3856ad364e35" newVersion="5.2.1.0" culture="neutral"/>
  </xsl:variable>

  <xsl:variable name="assemblies" select="msxsl:node-set($assemblies.tf)"/>

  <msxsl:script implements-prefix="safunc" language="C#">
    <![CDATA[
		public bool IsHigherVersion(string currentVersion, string newVersion)
		{
			var currentVer = Version.Parse(currentVersion);
			
			var newVer = Version.Parse(newVersion);
			
			return newVer > currentVer;
		}
		
		public string UpdateOldVersion(string oldVersion, string newVersion)
		{
			string[] versions;
			
			if (oldVersion.Contains("-"))
			{
				versions = oldVersion.Split('-');
			}
			else
			{
				versions = new[] { oldVersion, oldVersion };
			}
		
			var oldVer = Version.Parse(versions[1]);
			
			var newVer = Version.Parse(newVersion);
			
			if (newVer > oldVer)
			{
				versions[1] = newVer.ToString();
			}
			
			var x = string.Join("-", versions);
			
			return x;
		}
		]]>
  </msxsl:script>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

  <!-- missing runtime node -->
  <xsl:template match="/configuration[not(runtime)]">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <runtime>
        <xsl:text>&#13;&#10;</xsl:text>
        <xsl:call-template name="addAssemblyBindings"/>
        <xsl:text>&#13;&#10;</xsl:text>
      </runtime>
      <xsl:text>&#13;&#10;</xsl:text>
    </xsl:copy>
  </xsl:template>

  <!-- has runtime node but is missing assemblyBinding -->
  <xsl:template match="/configuration/runtime[not(msasm:assemblyBinding)]">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <xsl:call-template name="addAssemblyBindings"/>
      <xsl:text>&#13;&#10;</xsl:text>
    </xsl:copy>
  </xsl:template>

  <!-- has 0 or more elements under the assemblyBinding node -->
  <xsl:template match="/configuration/runtime/msasm:assemblyBinding">
    <xsl:copy>
      <xsl:for-each select="@* | node()">
        <xsl:variable name="assemblyName" select="msasm:assemblyIdentity/@name"/>
        <xsl:variable name="newAssemblyNode" select="$assemblies/assembly[@name=$assemblyName]"/>

        <xsl:choose>
          <xsl:when test="boolean($newAssemblyNode)">
            <xsl:variable name="currentNewVersion" select="msasm:bindingRedirect/@newVersion"/>
            <xsl:variable name="newVersion" select="$newAssemblyNode/@newVersion"/>

            <xsl:choose>
              <!-- check to see if the version being installed is higher than the one already installed -->
              <xsl:when test="safunc:IsHigherVersion($currentNewVersion, $newVersion)">
                <!-- replace existing one -->
                <xsl:call-template name="addDependentAssembly">
                  <xsl:with-param name="assemblyNode" select="$newAssemblyNode"/>
                </xsl:call-template>
              </xsl:when>
              <xsl:otherwise>
                <!-- keep existing one -->
                <xsl:copy>
                  <xsl:apply-templates select="@* | node()" />
                </xsl:copy>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:when>
          <xsl:otherwise>
            <xsl:copy>
              <xsl:apply-templates select="@* | node()" />
            </xsl:copy>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:for-each>

      <xsl:variable name="assemblyBindingRoot" select="."/>

      <!-- process the list of assemblies to add to find any that were not already found above -->
      <xsl:for-each select="$assemblies/assembly">
        <xsl:variable name="name" select="@name"/>
        <xsl:variable name="assemblyIdentityNode" select="$assemblyBindingRoot/msasm:dependentAssembly/msasm:assemblyIdentity[@name=$name]"/>

        <xsl:if test="count($assemblyIdentityNode)=0">
          <!-- add new one -->
          <xsl:call-template name="addDependentAssembly">
            <xsl:with-param name="assemblyNode" select="."/>
          </xsl:call-template>
        </xsl:if>
      </xsl:for-each>
    </xsl:copy>
  </xsl:template>

  <xsl:template name="addAssemblyBindings">
    <xsl:element name="assemblyBinding" namespace="urn:schemas-microsoft-com:asm.v1">
      <xsl:for-each select="$assemblies/assembly">
        <xsl:text>&#13;&#10;</xsl:text>
        <xsl:call-template name="addDependentAssembly">
          <xsl:with-param name="assemblyNode" select="."/>
        </xsl:call-template>
      </xsl:for-each>
      <xsl:text>&#13;&#10;</xsl:text>
    </xsl:element>
  </xsl:template>

  <xsl:template name="addDependentAssembly">
    <xsl:param name="assemblyNode"/>

    <xsl:element name="dependentAssembly" namespace="urn:schemas-microsoft-com:asm.v1">
      <xsl:text>&#13;&#10;</xsl:text>
      <xsl:element name="assemblyIdentity" namespace="urn:schemas-microsoft-com:asm.v1">
        <xsl:attribute name="name">
          <xsl:value-of select="$assemblyNode/@name"/>
        </xsl:attribute>
        <xsl:attribute name="publicKeyToken">
          <xsl:value-of select="$assemblyNode/@publicKeyToken"/>
        </xsl:attribute>
        <xsl:if test="normalize-space($assemblyNode/@culture)">
          <xsl:attribute name="culture">
            <xsl:value-of select="$assemblyNode/@culture"/>
          </xsl:attribute>
        </xsl:if>
      </xsl:element>
      <xsl:text>&#13;&#10;</xsl:text>
      <xsl:element name="bindingRedirect" namespace="urn:schemas-microsoft-com:asm.v1">
        <xsl:attribute name="oldVersion">
          <xsl:choose>
            <xsl:when test="normalize-space($assemblyNode/@oldVersion)">
              <xsl:value-of select="$assemblyNode/@oldVersion"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="concat('0.0.0.0-', $assemblyNode/@newVersion)"/>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <xsl:attribute name="newVersion">
          <xsl:value-of select="$assemblyNode/@newVersion"/>
        </xsl:attribute>
      </xsl:element>
      <xsl:text>&#13;&#10;</xsl:text>
    </xsl:element>
  </xsl:template>
</xsl:stylesheet>
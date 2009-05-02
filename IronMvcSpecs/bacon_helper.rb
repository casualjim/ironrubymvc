$: << File.dirname(__FILE__) + '/bin'
require 'bacon'
require 'mscorlib'
require File.dirname(__FILE__) + "/lib/amok.rb"

#load_assembly 'System.Web.Mvc.IronRuby, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
load_assembly 'System.Web.Routing, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35'
load_assembly 'System.Web.Mvc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35'
load_assembly 'System.Web.Mvc.IronRuby'
load_assembly 'BugWorkarounds'

include System::Web::Routing
include System::Web::Mvc
include System::Collections::Generic
include BugWorkarounds

IRCONTROLLER = System::Web::Mvc::IronRuby::Controllers
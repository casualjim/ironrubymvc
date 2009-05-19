$: << File.dirname(__FILE__) + '/bin'
require 'bacon'
require 'mscorlib'
require 'caricature'
require 'caricature/clr'

#load_assembly 'System.Web.Mvc.IronRuby, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
load_assembly 'System.Web.Routing, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35'
load_assembly 'System.Web.Mvc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35'
load_assembly 'System.Web.Mvc.IronRuby'
load_assembly 'BugWorkarounds'
load_assembly 'IronRuby, Version=0.4.0.0, Culture=neutral, PublicKeyToken=null'
load_assembly 'IronRuby.Libraries, Version=0.4.0.0, Culture=neutral, PublicKeyToken=null'

include System::Web::Routing
include System::Web::Mvc
include System::Collections::Generic
include IronRubyMvcWorkarounds
include IronRuby

IRCONTROLLER = System::Web::Mvc::IronRuby::Controllers unless defined? IRCONTROLLER
IRCORE = System::Web::Mvc::IronRuby::Core unless defined? IRCORE
IRVIEW = System::Web::Mvc::IronRuby::ViewEngine unless defined? IRVIEW
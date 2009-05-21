$: << File.dirname(__FILE__) + '/bin'
require 'bacon'
require 'mscorlib'
require 'caricature'
require 'caricature/clr'
require 'caricature/clr/aspnet_mvc'

#load_assembly 'System.Web.Mvc.IronRuby, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'

load_assembly 'System.Web.Mvc.IronRuby'
load_assembly 'BugWorkarounds'
load_assembly 'IronRuby, Version=0.4.0.0, Culture=neutral, PublicKeyToken=null'
load_assembly 'IronRuby.Libraries, Version=0.4.0.0, Culture=neutral, PublicKeyToken=null'

include System::Web
include System::Web::Routing
include System::Web::Mvc
include System::Collections::Generic
include System::Collections::Specialized

include IronRubyMvcWorkarounds

IRCONTROLLER = System::Web::Mvc::IronRuby::Controllers unless defined? IRCONTROLLER
IRCORE = System::Web::Mvc::IronRuby::Core unless defined? IRCORE
IRVIEW = System::Web::Mvc::IronRuby::ViewEngine unless defined? IRVIEW

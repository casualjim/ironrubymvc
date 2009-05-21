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

def debugger
    System::Diagnostics::Debugger.break if System::Diagnostics::Debugger.launch
end

class Object

  class << self

    def create_from_hash(options)
      result = self.new
      result.populate_from_hash options
      result
    end

  end

  def populate_from_hash(options)
    options.each do |k, v|
      mn = "#{k}=".to_sym
      self.send(mn, v) if self.respond_to?(mn)
    end
  end
end


shared "with ironruby initialized" do
  before do
    unless defined? @@script_runtime
      @@script_runtime = BugWorkarounds.create_script_runtime
    end

    @script_engine = BugWorkarounds.get_ruby_engine @@script_runtime
    @context = BugWorkarounds.get_execution_context @script_engine
  end
end

shared "with an engine initialized" do

  behaves_like "with ironruby initialized"

  before do
    @path_provider = Caricature::Isolation.for(IRCORE::IPathProvider)
    @path_provider.when_receiving(:application_physical_path).return(System::Environment.current_directory)
    @engine = IRCORE::RubyEngine.new @@script_runtime, @path_provider
  end

end

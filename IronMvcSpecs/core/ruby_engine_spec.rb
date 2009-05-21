require File.dirname(__FILE__) + "/../bacon_helper.rb"

def add_class_in(engine, context, class_name)
  puts "#{class_name}"
  script = "class #{class_name}; def my_method; $text_var = \"{0}\"; end; def another_method; $text_var = 'from other method'; end; end "
  context.define_global_variable("text_var", "String value")

  engine.execute_script script.to_clr_string
end

describe "RubyEngine" do

  # Called before each example.
  before do


  end

  # Called after each example.
  after do
    # Do nothing    
  end


  describe "when asked to initialize IronRubyMvc with an existing routes file" do
    before do
      @path_provider = Caricature::Isolation.for(IRCORE::IPathProvider)
      @path_provider.when_receiving(:application_physical_path).return(System::Environment.current_directory)
      @path_provider.when_receiving(:file_exists).return(true)
      @path_provider.when_receiving(:open).return(System::IO::MemoryStream.new(System::Byte[].new(0)))
      @script_engine = IRCORE::RubyEngine.initialize_iron_ruby_mvc @path_provider, "~/routes.rb"
      @routes = @script_engine.method(:get_global_variable).call("routes")
    end

    it "should have a global routes variable" do
      @routes.should.not.be.nil
    end

    it "should have a routes variable that is a RubyRoutes collection" do
      @routes.is_a?(IRCORE::RubyRoutes).should.be.true?
    end

    it "should have a ruby controller factory" do
      ControllerBuilder.current.get_controller_factory.is_a?(IRCONTROLLER::RubyControllerFactory).should.be.true?
    end

    it "should have a ruby view engine" do
      ViewEngines.engines.any? { |eng| eng.is_a?(IRVIEW::RubyViewEngine) }.should.be.true?
    end
    
  end

  describe "when asked to initialize ironruby mvc with a non-existing routes file" do
    before do
      @path_provider = Caricature::Isolation.for(IRCORE::IPathProvider)
      @path_provider.when_receiving(:application_physical_path).return(System::Environment.current_directory)
      @path_provider.when_receiving(:file_exists).return(false)
      @script_engine = IRCORE::RubyEngine.initialize_iron_ruby_mvc @path_provider, "~/routes.rb"

    end

    it "should have not a global routes variable" do
      lambda { @script_engine.method(:get_global_variable).call("routes") }.should.raise?(System::MissingMemberException)
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

  describe "when asked for an existing ruby class" do

    behaves_like "with an engine initialized"

    it "should be able to get the ruby class" do
      add_class_in @engine, @context, "SamuraiController"
      klass = @engine.get_ruby_class "SamuraiController"

      klass.name.should.match /^SamuraiController/
    end


  end

  describe "when asked to execute script" do

    behaves_like "with an engine initialized"

    it "should be able to execute script" do
      @engine.execute_script("\"It works from script\".to_clr_string").should.equal "It works from script"
    end

  end


end
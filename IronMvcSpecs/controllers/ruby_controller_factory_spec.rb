require File.dirname(__FILE__) + "/../bacon_helper.rb"

describe "RubyControllerFactory" do

  before do
    @cname = "addresses"
    @engine = Caricature::Isolation.for IRCORE::IRubyEngine
    @factory = Caricature::Isolation.for IControllerFactory
    @path_provider = Caricature::Isolation.for IRCORE::IPathProvider
    @controller = Caricature::Isolation.for IRCONTROLLER::RubyController
    @sut = IRCONTROLLER::RubyControllerFactory.new @path_provider, @factory, @engine
  end

  describe "when disposing a controller" do

    before do
      @sut.release_controller @controller
    end

    it "should have called dispose" do
      @controller.did_receive?(:dispose).should.be.successful
    end

  end

  describe "when resolving a ruby controller" do

    before do
      request_context = RequestContext.new http_context_isolation, RouteData.new
      fname = "~\\Controllers\\addresses_controller.rb"
      @engine.when_receiving(:load_controller).return(@controller)
      @path_provider.when_receiving(:file_exists).with("~\\Controllers\\AddressesController.rb").return(false)
      @path_provider.when_receiving(:file_exists).with(fname).return(true)
      @engine.when_receiving(:get_ruby_class).return(@controller.class)
      @engine.when_receiving(:create_instance).return(@controller)

      @factory.when_receiving(:create_controller).raise(System::InvalidOperationException.new)
      @result = @sut.create_controller(request_context, @cname)
    end

    it "should return a result" do
      @result.should.not.be.nil
    end

    it "should return a controller" do
      @result.is_a?(IController).should.be.true?
    end

    it "should have called the inner controller factory" do
      @factory.did_receive?(:create_controller).should.be.successful
    end

    it "should have asked the path provider if the file exists" do
      @path_provider.did_receive?(:file_exists).should.be.successful
    end

    it "should have asked the path provider if the controller file exists with a pascal cased name" do
      @path_provider.did_receive?(:file_exists).with("~\\Controllers\\AddressesController.rb".to_clr_string).should.be.successful
    end

    it "should have asked the path provider if the controller file exists with a snake cased name" do
      @path_provider.did_receive?(:file_exists).with("~\\Controllers\\addresses_controller.rb".to_clr_string).should.be.successful
    end

  end

end
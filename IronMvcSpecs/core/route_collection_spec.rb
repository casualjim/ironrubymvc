require File.dirname(__FILE__) + "/../bacon_helper.rb"

include System::Web::Mvc::IronRuby::Core

describe "RubyRoutes" do

  # Called before each example.
  before do
    handler = MvcRouteHandler.new
    route = Route.new "my_controller_url", handler
    route.constraints = RouteValueDictionary.new

    @expected = RouteCollection.new
    @expected.add "my_controller", route

  end

  # Called after each example.
  after do
    # Do nothing
  end

  describe "when mapping a single value" do

    it "should have a mapping" do
      actual = RubyRoutes.new RouteCollection.new
      actual.map_route "my_controller", "my_controller_url"

      actual['my_controller'].url.should.be == @expected['my_controller'].url
    end
    
  end

  describe "when multiple values are mapped" do

    before do
      @mappings = RubyRoutes.new RouteCollection.new
      @mappings.map_route "my_controller", "my_controller_url", {}
      @mappings.map_route "my_controller2", "my_controller_url2"
      @mappings.map_route "my_controller3", "my_controller_url3"
    end

    it "should have the third mapping" do
      @mappings['my_controller3'].url.should.be == "my_controller_url3"
    end

    it "should have the first mapping" do

      @mappings['my_controller'].url.should.be == @expected['my_controller'].url

    end

  end

end
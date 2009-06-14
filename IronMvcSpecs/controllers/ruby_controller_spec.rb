require File.dirname(__FILE__) + '/../bacon_helper.rb'

def create_controller_class(engine, name)
  script = <<SCRIPT
class #{name} < Controller
  def my_action
    "Can't see ninjas"
  end
end
SCRIPT
  engine.execute_script script.to_clr_string
end

describe "RubyController" do

  behaves_like "with an engine initialized"

  before do
    create_controller_class @engine, "SomeController"
    @http_context = http_context_isolation
    @request_context = RequestContext.new @http_context, RouteData.new
    klass = @engine.get_ruby_class "SomeController"
    @controller = @engine.create_instance klass
    @controller.internal_initialize(
            IRCONTROLLER::ControllerConfiguration.create_from_hash(
                    :context => @request_context,
                    :engine => @engine,
                    :ruby_class => klass
            )
    )
  end

  it "should have the correct controller class name" do
    @controller.controller_class_name.should.equal "SomeController"
  end

  it "should have the correct controller name" do
    @controller.controller_name.should.equal "Some"
  end

  describe "when asked for the params" do
    before do
      form, querystring = NameValueCollection.new, NameValueCollection.new
      form.add "a_form_field", "a form value"
      querystring.add "a_query_string_field", "a query string value"

      @http_context.request.when_receiving(:form).return(form)
      @http_context.request.when_receiving(:query_string).return(querystring)

      rd = @controller.controller_context.route_data.values
      rd.clear
      rd.add "route_data_field", "a route data value"
      @params = @controller.params
    end

    it "should have params" do
      @params.should.not.be.nil
    end

    it "should have a form field" do
      @params[:a_form_field].should.not.be.nil
    end

    it "should have a query string value" do
      @params[:a_query_string_field].should.not.be.nil
    end

    it "should have a route data field" do
      @params[:route_data_field].should.not.be.nil
    end

    it "should have the correct form field value" do
      @params[:a_form_field].should.equal "a form value"
    end

    it "should have the correct query string value" do
      @params[:a_query_string_field].should.equal "a query string value"
    end

    it "should have the correct route data value" do
      @params[:route_data_field].should.equal "a route data value"
    end

  end

  describe "when asked to redirect to route" do

    before do
      @result = @controller.redirect_to_route({ "MyRoute" => "RouteValue" })
    end

    it "should return a result" do
      @result.should.not.be.nil
    end

    it "should return a redirect to route result" do
      @result.is_a?(RedirectToRouteResult).should.be.true?
    end

    it "should return the correct redirect to route result" do
      @result.route_values["MyRoute"].should.not.be.nil
      @result.route_values["MyRoute"].should.equal "RouteValue"
    end

  end

  describe "when asked to redirect to action" do

    before do
      @result = @controller.redirect_to_action "my_action", "MyRoute" => "RouteValue"
    end

    it "should return a result" do
      @result.should.not.be.nil
    end

    it "should return a redirect to route result" do
      @result.is_a?(RedirectToRouteResult).should.be.true?
    end

    it "should return the correct redirect to route result" do
      @result.route_values["MyRoute"].should.not.be.nil
      @result.route_values["MyRoute"].should.equal "RouteValue"
    end

    it "should have the correct action name" do
      @result.route_values["action"].should.equal "my_action"
    end

  end

  describe "when asked for view data" do

    before do
      @result = @controller.view_data
    end

    it "should return a result" do
      @result.should.not.be.nil
    end

    it "should return a redirect to action result" do
      @result.is_a?(Dictionary.of(System::Object, System::Object)).should.be.true?
    end

  end

  describe "when asked to execute an action" do

    before do
      form, querystring = NameValueCollection.new, NameValueCollection.new
      @http_context.request.when_receiving(:form).return(form)
      @http_context.request.when_receiving(:query_string).return(querystring)
      @http_context.session.when_receiving(:[]).return({})
    end

    it "should not raise any errors" do
      controller = @controller
      request_context = @request_context

      lambda { |*args| controller.execute request_context }.should.not.raise
    end
  end

  shared "view context" do
    it "should return a view" do
      @result.should.not.be.nil
    end

    it "should have view data" do
      @result.view_data.should.not.be.nil
    end

    it "should have temp data" do
      @result.temp_data.should.not.be.nil
    end
  end

  describe "when asked for a view" do

    before do
      @result = @controller.view
    end

    behaves_like "view context"

  end

  describe "when asked for a view by name" do

    before do
      @result = @controller.view "my_action"
    end

    behaves_like "view context"

    it "should have a view name" do
      @result.view_name.should.equal "my_action"
    end

  end

  describe "when asked for a view with an object" do

    before do
      @view_item = Object.new
      @result = @controller.view @view_item
    end

    behaves_like "view context"

    it "should have the right view data" do
      @result.view_data.model.should.be.same_as @view_item
    end

    it "should have the correct temp data" do
      @result.temp_data.should.be.same_as @controller.temp_data
    end

  end

  describe "when asked for a view with name and master" do

    before do
      @result = @controller.view "my_action", "the_master"
    end

    behaves_like "view context"

    it "should have the correct temp data" do
      @result.temp_data.should.be.same_as @controller.temp_data
    end

    it "should have a view name" do
      @result.view_name.should.equal "my_action"
    end

    it "should have a master name" do
      @result.master_name.should.equal "the_master"
    end

  end

  describe "when asked for a view with name and object" do
    before do
      @view_item = Object.new
      @result = @controller.view "my_action", @view_item
    end

    behaves_like "view context"

    it "should have the correct temp data" do
      @result.temp_data.should.be.same_as @controller.temp_data
    end

    it "should have a view name" do
      @result.view_name.should.equal "my_action"
    end

    it "should have the right view data" do
      @result.view_data.model.should.be.same_as @view_item
    end

  end

end
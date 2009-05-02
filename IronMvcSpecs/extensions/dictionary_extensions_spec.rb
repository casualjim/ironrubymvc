require File.dirname(__FILE__) + "/extensions.rb"

def build_dictionary(klass)
  result = klass.new
  result.add "first", "first_action"
  result.add "second", "second action"
  result
end

describe "DictionaryExtensions" do

  describe "When converting existing values" do

    before do
      @hash = { "first" => "first_action", "second" => "second action" }
    end

    it "should convert a hash to a route dictionary" do
      expected = build_dictionary(RouteValueDictionary)

      actual = IDictionaryExtensions.to_route_dictionary @hash

      expected.each do |pair|
        pair.value.should.be == actual[pair.key]
      end
    end

    it "should convert a hash to a route dictionary" do
      expected = build_dictionary(ViewDataDictionary)

      actual = IDictionaryExtensions.to_view_data_dictionary @hash

      expected.each do |pair|
        pair.value.should.be == actual[pair.key]
      end
    end

    it "should convert a hash to a regular dictionary" do
      expected = build_dictionary(Dictionary.of(System::String, System::Object))

      actual = IDictionaryExtensions.to_dictionary @hash

      expected.each do |pair|
        pair.value.should.be == actual[pair.key]
      end
    end

  end

end
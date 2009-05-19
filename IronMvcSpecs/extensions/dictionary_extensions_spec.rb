require File.dirname(__FILE__) + "/extensions.rb"

def build_dictionary(klass)
  result = klass.new
  result.add "first", "first_action"
  result.add "second", "second action"
  result
end

describe "DictionaryExtensions" do

  before do
    @hash = { "first" => "first_action", "second" => "second action" }
  end

  describe "When converting existing values" do

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

  describe "iterating" do

    it "should iterate over a hash" do
      counter = 0

      p = Proc.new { |key, value| counter += 1 }
      IDictionaryExtensions.for_each(@hash, BugWorkarounds.wrap_proc2(p))

      counter.should.be == 2
    end

  end

end
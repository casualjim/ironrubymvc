require File.dirname(__FILE__) + "/extensions.rb"

class TestActionFilter < IRCONTROLLER::RubyActionFilter; end;
class TestAuthorizationFilter < IRCONTROLLER::RubyAuthorizationFilter; end;
class TestExceptionFilter < IRCONTROLLER::RubyExceptionFilter; end;
class TestResultFilter < IRCONTROLLER::RubyResultFilter; end;

describe "FilterInfoExtensions" do

  # Called before each example.
  before do
    # Do nothing
  end

  # Called after each example.
  after do
    # Do nothing
  end

  it "should merge 2 filter infos" do
    fi1, fi2 = FilterInfo.new, FilterInfo.new

    fi1.action_filters.add TestActionFilter.new
    fi1.authorization_filters.add TestAuthorizationFilter.new
    fi1.exception_filters.add TestExceptionFilter.new
    fi1.result_filters.add TestResultFilter.new

    fi2.action_filters.add TestActionFilter.new
    fi2.authorization_filters.add TestAuthorizationFilter.new
    fi2.exception_filters.add TestExceptionFilter.new
    fi2.result_filters.add TestResultFilter.new

    FilterInfoExtensions.merged_with(fi1, fi2)

    fi1.action_filters.count.should.be == 2
    fi1.authorization_filters.count.should.be == 2
    fi1.exception_filters.count.should.be == 2
    fi1.result_filters.count.should.be == 2
    
  end
end
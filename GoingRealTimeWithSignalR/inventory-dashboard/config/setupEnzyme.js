const configure = require('enzyme').configure;
const EnzymeAdapter = require('enzyme-adapter-react-16');

configure({ adapter: new EnzymeAdapter() });
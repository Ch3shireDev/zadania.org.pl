

export enum DataType {
  Int,
  Double
}

export class Exercise {
  public id: string;
  public title: string;

  public content: string;

  public variableData: { name, description, expression, type; }[] = [];
  public answerData: { name, description, expression, type; }[] = [];


  public initialize(): [string, any] {

    const variables = this.getVariables();
    const content = this.getContent(variables);
    const answers = this.getAnswers(variables);

    return [content, answers];
  }

  public getContent(variables = null): string {

    if (variables === null) {
      variables = this.getVariables();
    }

    let content = this.content;

    if (content === undefined) { return ''; }

    const matches = content.match(/\$\w+/g);

    if (matches) {
      matches.forEach(match => {
        const variable = match.replace('$', '');
        if (variable in variables) {
          content = content.replace(match, variables[variable]);
        }
      });
    }
    return content;
  }

  getVariables(): object {
    const variables = {};
    this.variableData.forEach(variable => {
      variables[variable.name] = eval(variable.expression);
    });
    return variables;
  }

  public evaluate(expression: string, values: object): string {

    const found = expression.match(/\$\w+/g);
    let isSafe = true;
    found.forEach(x => {
      x = x.replace('$', '');
      if (!(x in values)) {
        isSafe = false;
      }
    });

    if (!isSafe) {
      return undefined;
    }

    found.forEach(x => {
      const y = x.replace('$', '');
      expression = expression.replace(x, values[y]);
    });

    try {
      return eval(expression);
    }
    catch {
      return undefined;
    }

    return undefined;
  }

  public getAnswers(variables: object): { name, description, value, userValue; }[] {
    const questions = [];
    this.answerData.forEach(answer => {
      const value = this.evaluate(answer.expression, variables);
      questions.push({ name: answer.name, description: answer.description, value, userValue: undefined });
    });
    return questions;
  }

}

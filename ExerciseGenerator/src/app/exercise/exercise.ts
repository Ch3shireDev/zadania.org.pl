

export enum DataType {
  Int,
}

export class Exercise {

  public id: string;
  public title: string;

  public content: string;
  public variableData: { name, expression, type; }[] = [];

  public answerData: { name, expression, type; }[] = [];

  public getContent(variables): string {
    let content = this.content;
    const matches = content.match(/\$\w+/g);
    if (matches) {
      matches.forEach(match => {
        content = content.replace(match, variables[match.replace('$', '')]);
      });
    }
    return content;
  }

  getVariables(): object {
    let variables = {};
    this.variableData.forEach(variable => {
      variables[variable.name] = eval(variable.expression);
    });
    return variables;
  }

  public evaluate(expression: string, values: object) {

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

  public getAnswers(variables: object): { name, value, userAnswer; }[] {
    const questions = [];
    this.answerData.forEach(answer => {
      const value = this.evaluate(answer.expression, variables);
      questions.push({ name: answer.name, value, userAnswer: undefined });
    });
    return questions;
  }

}

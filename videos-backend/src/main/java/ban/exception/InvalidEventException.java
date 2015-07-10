package ban.exception;

import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.ResponseStatus;

/**
 * Created by bnorrish on 7/10/15.
 */
@ResponseStatus(value = HttpStatus.BAD_REQUEST, reason="Invalid Event")
public class InvalidEventException extends Exception {

  public InvalidEventException(String message) {
    super(message);
  }
}

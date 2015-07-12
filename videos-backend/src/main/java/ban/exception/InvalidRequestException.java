package ban.exception;

import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.ResponseStatus;

/**
 * Created by bnorrish on 7/12/15.
 */
@ResponseStatus(value = HttpStatus.BAD_REQUEST, reason="Invalid Request")
public class InvalidRequestException extends RuntimeException {
}
